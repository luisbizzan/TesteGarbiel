using FWLog.Services.Helpers;
using System;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace FWLog.Services.IntegracaoSankhya
{
    public sealed class SankhyaLogin
    {
        private static readonly object padlock = new object();
        private readonly string xmlLogin = "<serviceRequest serviceName='MobileLoginSP.login'><requestBody><NOMUSU>{0}</NOMUSU><INTERNO>{1}</INTERNO></requestBody></serviceRequest>";
        private static SankhyaLogin _instace;
        private string Token { get; set; }
        private DateTime UltimaAutenticacao { get; set; }

        public static SankhyaLogin Instance
        {
            get
            {
                lock (padlock)
                {
                    return _instace ?? (_instace = new SankhyaLogin());
                }
            }
        }

        public string GetToken()
        {
            if (DateTime.UtcNow - UltimaAutenticacao > TimeSpan.FromMinutes(4))
            {
                Token = Instance.Login("dartdigital", "123dart");
            }

            return Token;
        }

        private string Login(string user, string password)
        {
            var contentLogin = new StringContent(string.Format(xmlLogin, user, password), Encoding.UTF8, "text/xml");
            string uriLogin = "http://10.1.21.9:8380/mge/service.sbr?serviceName=MobileLoginSP.login";

            HttpResponseMessage login = HttpService.Instance.PostAsync(uriLogin, contentLogin).Result;

            if (!login.IsSuccessStatusCode)
            {
                //TODO lançar exception de não autorizados sankhya
            }

            string responseContent = login.Content.ReadAsStringAsync().Result;

            XDocument doc = XDocument.Parse(responseContent);
            XElement root = doc.Root;

            string status = root.Attribute("status")?.Value;
            if (status != "1")
            {
                //TODO lançar exception de não autorizados sankhya
            }

            string jsessionid = root.Element("responseBody").Element("jsessionid")?.Value;
            if (jsessionid == null)
            {
                //TODO lançar exception de não autorizados sankhya
            }

            UltimaAutenticacao = DateTime.UtcNow;

            return jsessionid;
        }
    }
}
