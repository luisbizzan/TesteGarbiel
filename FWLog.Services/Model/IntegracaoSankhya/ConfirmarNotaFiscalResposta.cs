using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{

    [XmlRoot("serviceResponse")]
    public class ConfirmarNotaFiscalResposta
    {
        public ConfirmarNotaFiscalResposta()
        {
            CorpoResposta = new ConfirmarNotaFiscalCorpo();
        }

        [XmlElement("responseBody")]
        public ConfirmarNotaFiscalCorpo CorpoResposta { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }
    }

    public class ConfirmarNotaFiscalCorpo
    {
        public ConfirmarNotaFiscalCorpo()
        {
            ChavePrimaria = new ConfirmarNotaFiscalValor();
        }

        [XmlElement("pk")]
        public ConfirmarNotaFiscalValor ChavePrimaria { get; set; }
    }

    public class ConfirmarNotaFiscalValor
    {
        public ConfirmarNotaFiscalValor() { }

        [XmlElement("NUNOTA")]
        public long CodigoIntegracao { get; set; }
    }
}