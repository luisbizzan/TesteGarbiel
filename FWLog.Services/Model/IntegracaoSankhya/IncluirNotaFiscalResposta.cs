using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot("serviceResponse")]
    public class IncluirNotaFiscalResposta
    {
        [XmlElement("responseBody")]
        public IncluirNotaRespostaCorpo CorpoResposta { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }
    }

    public class IncluirNotaRespostaCorpo
    {
        [XmlElement("pk")]
        public IncluirNotaRespostaValor ChavePrimaria { get; set; }
    }

    public class IncluirNotaRespostaValor
    {
        [XmlElement("NUNOTA")]
        public string CodigoIntegracao{ get; set; }
    }
}