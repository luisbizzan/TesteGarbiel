using FWLog.Services.Integracao.Helpers;
using FWLog.Services.Model.IntegracaoSankhya;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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

            string uriLogin = string.Concat(BaseURL, "mge/service.sbr?serviceName=MobileLoginSP.login");

            HttpResponseMessage login = HttpService.Instance.PostAsync(uriLogin, contentLogin).Result;

            if (!login.IsSuccessStatusCode)
            {
                throw new Exception("O sistema não obteve sucesso na autorização da Integração Sankhya.");
            }

            string responseContent = login.Content.ReadAsStringAsync().Result;

            LoginReposta resposta = DeserializarXML<LoginReposta>(responseContent);

            if (resposta.CorpoResposta == null || resposta.Status != "1" || resposta.CorpoResposta.Token == null)
            {
                var erro = DeserializarXML<IntegracaoErroResposta>(responseContent);

                throw new Exception(string.Format("O sistema não obteve o jsessionid na autorização da Integração Sankhya. Mensagem de Erro {0}", erro.Mensagem));
            }

            UltimaAutenticacao = DateTime.UtcNow;

            return resposta.CorpoResposta.Token;
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

        public TClass DeserializarXML<TClass>(string xml) where TClass : class, new()
        {
            TClass resposta = null;

            XmlSerializer serializer = new XmlSerializer(typeof(TClass));
            using (TextReader reader = new StringReader(xml))
            {
                resposta = (TClass)serializer.Deserialize(reader);
            }

            return resposta;
        }

        private string SerializarXML<TClass>(TClass root)
        {
            var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            XmlSerializer serializer = new XmlSerializer(root.GetType());
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            StringBuilder stringXML = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringXML, settings))
            {
                serializer.Serialize(xmlWriter, root, emptyNs);
            }

            return stringXML.ToString();
        }

        public async Task<string> ExecutarServicoSankhya(string xml, string serviceName, string service)
        {
            StringContent contentString = new StringContent(xml, Encoding.UTF8, "text/xml");

            contentString.Headers.Add("Cookie", string.Format("JSESSIONID={0}", Instance.GetToken()));

            var uri = string.Format("{0}{1}/service.sbr?serviceName={2}&mgeSession={3}", BaseURL, service, serviceName, Instance.GetToken());

            HttpResponseMessage httpResponse = await HttpService.Instance.PostAsync(uri, contentString);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("O sistema obteve o status {0} na resposta do serviço 'CRUDServiceProvider.saveRecord' Integração Sankhya", httpResponse.StatusCode));
            }

            return httpResponse.Content.ReadAsStringAsync().Result;
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

            var uri = string.Format("{0}mge/service.sbr?serviceName=DbExplorerSP.executeQuery&mgeSession={1}", BaseURL, Instance.GetToken());

            HttpResponseMessage httpResponse = await HttpService.Instance.PostAsync(uri, contentString);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("O sistema obteve o status {0} na consulta da 'DbExplorerSP' Integração Sankhya", httpResponse.StatusCode));
            }

            string result = httpResponse.Content.ReadAsStringAsync().Result;

            return result;
        }

        public async Task<List<TClass>> PreExecutarQuery<TClass>(string where = "", string inner = "") where TClass : class, new()
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

            string resultado = await Instance.ExecuteQuery(sql);
            if (resultado == null)
            {
                return resultList;
            }

            ExecuteQueryResponse resultObj = null;

            try
            {
                resultObj = JsonConvert.DeserializeObject<ExecuteQueryResponse>(resultado);
            }
            catch (Exception e)
            {
                var erro = DeserializarXML<IntegracaoErroResposta>(resultado);

                throw new BusinessException(string.Format("Ocorreu um erro na consulta da tabela {0}. Mensagem de Erro {1}", classAttr.DisplayName, erro.Mensagem));
            }

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

        public async Task<bool> AtualizarInformacaoIntegracao(string entidade, string campoPKIntegracao, long valorPKIntegracao, string campo, object valor)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return false;
            }

            XElement dataRow = new XElement("dataRow", new XElement("localFields", new XElement(campo, valor)));
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

            string responseContent = await Instance.ExecutarServicoSankhya(serviceRequest.ToString(), "CRUDServiceProvider.saveRecord", "mge");

            XDocument doc = XDocument.Parse(responseContent);
            XElement root = doc.Root;

            string status = root.Attribute("status")?.Value;
            if (status == null || status != "1")
            {
                var erro = DeserializarXML<IntegracaoErroResposta>(root.ToString());
                throw new BusinessException(string.Format("O sistema não obteve sucesso na atualização da entidade {0}. Mensagem de Erro {1}", entidade, erro.Mensagem));
            }

            return true;
        }

        public async Task<bool> ConfirmarNotaFiscal(long condigoIntegracao)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return false;
            }

            string confirmarNotaXML = SerializarXML(new ConfirmarNotaFiscalXML(condigoIntegracao.ToString()));

            string rootXML = await Instance.ExecutarServicoSankhya(confirmarNotaXML, "CACSP.confirmarNota", "mgecom");

            ConfirmarNotaFiscalResposta resposta = DeserializarXML<ConfirmarNotaFiscalResposta>(rootXML);

            if (resposta.CorpoResposta == null || resposta.Status != "1" || resposta.CorpoResposta.ChavePrimaria?.CodigoIntegracao == null)
            {
                var erro = DeserializarXML<IntegracaoErroResposta>(rootXML.ToString());

                throw new BusinessException(string.Format("Ocorreu um erro na confirmação da nota fiscal número único {0}. Mensagem de Erro {1}", condigoIntegracao, erro.Mensagem));
            }

            return true;
        }

        public async Task<bool> IncluirNotaFiscal()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return false;
            }

            string incluirNotaXML = SerializarXML(new IncluirNotaFiscalXML());

            string rootXML = await Instance.ExecutarServicoSankhya(incluirNotaXML, "CACSP.incluirNota", "mgecom");

            IncluirNotaFiscalResposta resposta = DeserializarXML<IncluirNotaFiscalResposta>(rootXML);

            if (resposta.CorpoResposta == null || resposta.Status != "1" || resposta.CorpoResposta.ChavePrimaria?.CodigoIntegracao == null)
            {
                var erro = DeserializarXML<IntegracaoErroResposta>(rootXML.ToString());

                throw new Exception(string.Format("Ocorreu um erro na inclusão de uma nova nota fiscal da nota fiscal. Mensagem de Erro {0}", erro.Mensagem));
            }

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
