﻿using FWLog.Services.Integracao.Helpers;
using FWLog.Services.Model;
using FWLog.Services.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWLog.Services.Integracao
{
    public sealed class IntegracaoSankhya
    {
        private static readonly object padlock = new object();
        private readonly string xmlLogin = "<serviceRequest serviceName='MobileLoginSP.login'><requestBody><NOMUSU>{0}</NOMUSU><INTERNO>{1}</INTERNO></requestBody></serviceRequest>";
        private static IntegracaoSankhya _instace;
        private string Token { get; set; }
        private DateTime UltimaAutenticacao { get; set; }

        public static IntegracaoSankhya Instance
        {
            get
            {
                lock (padlock)
                {
                    return _instace ?? (_instace = new IntegracaoSankhya());
                }
            }
        }

        private string BaseURL
        {
            get
            {
                var url = ConfigurationManager.AppSettings["IntegracaoSankhya_URL"];
                if (url == null)
                {
                    throw new NullReferenceException("URL de Integração Sankhya");
                }

                return url;
            }
        }

        private string Senha
        {
            get
            {
                var value = ConfigurationManager.AppSettings["IntegracaoSankhya_Senha"];
                if (value == null)
                {
                    throw new NullReferenceException("Senha de Integração Sankhya");
                }

                return value;
            }
        }

        private string Usuario
        {
            get
            {
                var value = ConfigurationManager.AppSettings["IntegracaoSankhya_Usuario"];
                if (value == null)
                {
                    throw new NullReferenceException("Usuário de Integração Sankhya");
                }

                return value;
            }
        }

        private string Login()
        {
            var contentLogin = new StringContent(string.Format(xmlLogin, Usuario, Senha), Encoding.UTF8, "text/xml");

            string uriLogin = string.Concat(BaseURL, "serviceName=MobileLoginSP.login");

            HttpResponseMessage login = HttpService.Instance.PostAsync(uriLogin, contentLogin).Result;

            if (!login.IsSuccessStatusCode)
            {
                throw new Exception("O sistema não obteve sucesso na autorização da Integração Sankhya.");
            }

            string responseContent = login.Content.ReadAsStringAsync().Result;

            XDocument doc = XDocument.Parse(responseContent);
            XElement root = doc.Root;

            string status = root.Attribute("status")?.Value;
            if (status != "1")
            {
                throw new Exception("O sistema não obteve o status 1 na autorização da Integração Sankhya.");
            }

            string jsessionid = root.Element("responseBody").Element("jsessionid")?.Value;
            if (jsessionid == null)
            {
                throw new Exception("O sistema não obteve o jsessionid na autorização da Integração Sankhya.");
            }

            UltimaAutenticacao = DateTime.UtcNow;

            return jsessionid;
        }

        public string GetToken()
        {
            var expiracao = ConfigurationManager.AppSettings["IntegracaoSankhya_TempoExpiracaoToken"];
            if (expiracao == null)
            {
                throw new NullReferenceException("Tempo de Expiração do Token de Autenticação de Integração Sankhya");
            }

            if (DateTime.UtcNow - UltimaAutenticacao > TimeSpan.FromMinutes(Convert.ToInt32(expiracao)))
            {
                lock (padlock)
                {
                    Token = Instance.Login();
                }
            }

            return Token;
        }

        public async Task ExecutarSaveRecord(XElement xml)
        {
            StringContent contentString = new StringContent(xml.ToString(), Encoding.UTF8, "text/xml");

            contentString.Headers.Add("Cookie", string.Format("JSESSIONID={0}", Instance.GetToken()));

            var uri = string.Format("{0}serviceName=CRUDServiceProvider.saveRecord", BaseURL);

            HttpResponseMessage httpResponse = await HttpService.Instance.PostAsync(uri, contentString);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("O sistema obteve o status {0} na resposta do serviço 'CRUDServiceProvider.saveRecord' Integração Sankhya", httpResponse.StatusCode));
            }

            string result = httpResponse.Content.ReadAsStringAsync().Result;

            XDocument doc = XDocument.Parse(result);
            XElement root = doc.Root;

            string status = root.Attribute("status")?.Value;
            if (status != "1")
            {
                throw new Exception(string.Format("O sistema não obteve o status 1 no retorno da atualização da nota fiscal no Integração Sankhya. Mensagem {0}", root.Element("statusMessage")?.Value));
            }

            string nunota = root.Element("responseBody").Element("entities").Element("entity").Element("NUNOTA")?.Value;
            if (nunota == null)
            {
                throw new Exception("O sistema não obteve o campo NUNOTA no retorno da atualização da nota fiscal no Integração Sankhya.");
            }
        }

        public async Task<string> ExecuteQuery(string query)
        {
            var queryJson = new
            {
                serviceName = "DbExplorerSP.executeQuery",
                requestBody = new
                {
                    sql = query
                }
            };

            string jsonContent = JsonConvert.SerializeObject(queryJson);

            StringContent contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var uri = string.Format("{0}serviceName=DbExplorerSP.executeQuery&mgeSession={1}", BaseURL, Instance.GetToken());

            HttpResponseMessage httpResponse = await HttpService.Instance.PostAsync(uri, contentString);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("O sistema obteve o status {0} na consulta da 'DbExplorerSP' Integração Sankhya", httpResponse.StatusCode));
            }

            string result = httpResponse.Content.ReadAsStringAsync().Result;

            return result;
        }

        public async Task<List<TClass>> PreExecutarQueryGenerico<TClass>(string where = null) where TClass : class, new()
        {
            Type typeClass = typeof(TClass);
            List<TClass> resultList = null;

            TabelaIntegracaoAttribute classAttr = (TabelaIntegracaoAttribute)typeClass.GetCustomAttributes(typeof(TabelaIntegracaoAttribute), false).FirstOrDefault();

            if (classAttr == null)
            {
                return resultList;
            }

            var listColumns = new List<string>();

            PropertyInfo[] properties = typeClass.GetProperties();
            foreach (var propertyInfo in properties)
            {
                listColumns.Add(propertyInfo.Name);
            }

            var sqlColunas = string.Join(",", listColumns.ToArray());

            var sql = string.Format("SELECT {0} FROM {1} {2}", sqlColunas, classAttr.DisplayName, where);

            var resultJson = await Instance.ExecuteQuery(sql);
            if (resultJson == null)
            {
                return resultList;
            }

            ExecuteQueryResponse resultObj = JsonConvert.DeserializeObject<ExecuteQueryResponse>(resultJson);

            resultList = new List<TClass>();
            foreach (var row in resultObj.responseBody.rows)
            {
                var newClass = new TClass();
                for (var i = 0; i <= listColumns.Count() - 1; i++)
                {
                    PropertyInfo propertySet = typeClass.GetProperty(listColumns[i]);
                    propertySet.SetValue(newClass, row[i], null);
                }

                resultList.Add(newClass);
            }

            return resultList;
        }

        public async Task<List<TClass>> PreExecutarQueryComplexa<TClass>(string where = "", string inner = "") where TClass : class, new()
        {
            Type typeClass = typeof(TClass);
            List<TClass> resultList = null;

            TabelaIntegracaoAttribute classAttr = (TabelaIntegracaoAttribute)typeClass.GetCustomAttributes(typeof(TabelaIntegracaoAttribute), false).FirstOrDefault();

            if (classAttr == null)
            {
                return resultList;
            }

            var listColumns = new List<ColunasConsulta>();

            PropertyInfo[] properties = typeClass.GetProperties();
            foreach (var propertyInfo in properties)
            {
                TabelaIntegracaoAttribute queryProperty = (TabelaIntegracaoAttribute)propertyInfo.GetCustomAttributes(typeof(TabelaIntegracaoAttribute), false).FirstOrDefault();

                if (queryProperty == null)
                {
                    continue;
                }

                listColumns.Add(new ColunasConsulta(queryProperty.DisplayName, propertyInfo.Name));
            }

            var sqlColunas = string.Join(",", listColumns.Select(s => s.Coluna).ToArray());

            var sql = string.Format("SELECT {0} FROM {1} {2} {3}", sqlColunas, classAttr.DisplayName, inner, where);

            var resultJson = await Instance.ExecuteQuery(sql);
            if (resultJson == null)
            {
                return resultList;
            }

            ExecuteQueryResponse resultObj = JsonConvert.DeserializeObject<ExecuteQueryResponse>(resultJson);

            resultList = new List<TClass>();
            foreach (var row in resultObj.responseBody.rows)
            {
                var newClass = new TClass();
                for (var i = 0; i <= listColumns.Count() - 1; i++)
                {
                    PropertyInfo propertySet = typeClass.GetProperty(listColumns[i].Nome);
                    propertySet.SetValue(newClass, row[i], null);
                }

                resultList.Add(newClass);
            }

            return resultList;
        }

        public async Task<bool> AtualizarInformacaoIntegracao(string entidade, string campoPKIntegracao, long valorPKIntegracao, string campoStatus, object valorStatus)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return false;
            }

            XElement dataRow = new XElement("dataRow", new XElement("localFields", new XElement(campoStatus, valorStatus)));
            dataRow.Add(new XElement("key", new XElement(campoPKIntegracao, valorPKIntegracao)));

            XAttribute[] attArray = {
                new XAttribute("rootEntity", entidade),
                new XAttribute("includePresentationFields", "S"),
            };

            var entity = new XElement("entity", new XAttribute("path", ""));
            entity.Add(new XElement("fieldset", new XAttribute("list", "*")));

            XElement datset = new XElement("dataSet", attArray);
            datset.Add(entity);
            datset.Add(dataRow);

            XElement serviceRequest = new XElement("serviceRequest", new XAttribute("serviceName", "CRUDServiceProvider.saveRecord"));
            serviceRequest.Add(new XElement("requestBody", datset));

            await Instance.ExecutarSaveRecord(serviceRequest);

            return true;
        }
    }
}

public class ColunasConsulta
{
    public string Coluna { get; set; }
    public string Nome { get; set; }

    public ColunasConsulta(string coluna, string nome)
    {
        Coluna = coluna;
        Nome = nome;
    }
}
