using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot("serviceResponse")]
    public class DevolucaoRespostaXML
    {
        [XmlElement("responseBody")]
        public DevolucaoRespostaCorpo CorpoResposta { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }
    }

    public class DevolucaoRespostaCorpo
    {
        [XmlElement("notas")]
        public DevolucaoRespostaValor Notas { get; set; }
    }

    public class DevolucaoRespostaValor
    {
        [XmlElement("nota")]
        public long CodigoIntegracao { get; set; }
    }
}
