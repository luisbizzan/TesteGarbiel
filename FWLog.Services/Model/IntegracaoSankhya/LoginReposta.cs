using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot("serviceResponse")]
    public class LoginReposta
    {
        [XmlElement("responseBody")]
        public LoginRepostaCorpo CorpoResposta { get; set; }

         [XmlAttribute("status")]
        public string Status { get; set; }
    }

    public class LoginRepostaCorpo
    {
        [XmlElement("jsessionid")]
        public string Token { get; set; }

        [XmlElement("idusu")]
        public string Id { get; set; }

        [XmlElement("callID")]
        public string CallId { get; set; }
    }
}
