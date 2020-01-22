using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot("serviceRequest")]
    public class ConfirmarNotaFiscalXML
    {
        public ConfirmarNotaFiscalXML() { }
        public ConfirmarNotaFiscalXML(string codigoIntegracao)
        {
            Nome = "CACSP.confirmarNota";
            ElementoCorpo = new ElementoCorpo(codigoIntegracao);
        }

        [XmlAttribute("serviceName")]
        public string Nome { get; set; }

        [XmlElement("requestBody")]
        public ElementoCorpo ElementoCorpo { get; set; }
    }

    public class ElementoCorpo
    {
        public ElementoCorpo() { }
        public ElementoCorpo(string codigoIntegracao)
        {
            ElementoNota = new ElementoNota(codigoIntegracao);
        }

        [XmlElement("nota")]
        public ElementoNota ElementoNota { get; set; }
    }

    public class ElementoNota
    {
        public ElementoNota() { }
        public ElementoNota(string codigoIntegracao)
        {
            confirmacaoCentralNota = "true";
            atualizaPrecoItemPedCompra = "false";
            ehPedidoWeb = "false";
            ownerServiceCall = "CentralNotas";
            CodigoIntegracao = codigoIntegracao;
        }

        [XmlAttribute]
        public string confirmacaoCentralNota { get; set; }

        [XmlAttribute]
        public string ehPedidoWeb { get; set; }

        [XmlAttribute]
        public string atualizaPrecoItemPedCompra { get; set; }

        [XmlAttribute]
        public string ownerServiceCall { get; set; }

        [XmlElement("NUNOTA")]
        public string CodigoIntegracao { get; set; }
    }
}
