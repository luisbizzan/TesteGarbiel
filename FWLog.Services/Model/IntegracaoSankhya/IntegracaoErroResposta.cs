using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot("serviceResponse")]
    public class IntegracaoErroResposta
    {
        [XmlElement("tsError")]
        public string Error { get; set; }

        [XmlElement("statusMessage")]
        public string Mensagem { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }
    }
}