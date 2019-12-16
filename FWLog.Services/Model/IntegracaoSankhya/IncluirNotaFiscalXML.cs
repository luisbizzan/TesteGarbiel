using System.Collections.Generic;
using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot("serviceRequest")]
    public class IncluirNotaFiscalXML
    {
        public IncluirNotaFiscalXML()
        {
            Nome = "CACSP.confirmarNota";
            IncluirNFCorpo = new IncluirNFCorpo();
        }

        [XmlAttribute("serviceName")]
        public string Nome { get; set; }

        [XmlElement("requestBody")]
        public IncluirNFCorpo IncluirNFCorpo { get; set; }
    }

    public class IncluirNFCorpo
    {
        public IncluirNFCorpo()
        {
            IncluirNFNota = new IncluirNFNota();
        }

        [XmlElement("nota")]
        public IncluirNFNota IncluirNFNota { get; set; }
    }
    
    public class IncluirNFNota
    {
        public IncluirNFNota()
        {
            Cabecalho = new IncluirNFNotaCabecalho();
            Itens = new IncluirNFNotaItens();
        }

        [XmlElement("cabecalho")]
        public IncluirNFNotaCabecalho Cabecalho { get; set; }

        [XmlElement("itens")]
        public IncluirNFNotaItens Itens { get; set; }
    }

    public class IncluirNFNotaItens
    {
        public IncluirNFNotaItens()
        {
            ListaItens = new List<IncluirNFNotaItem>();
        }

        [XmlElement("item")]

        public List<IncluirNFNotaItem> ListaItens { get; set; }
    }

    public class IncluirNFNotaCabecalho
    {
        [XmlElement("NUNOTA")]
        public string CodigoIntegracaoNotaFiscal { get; set; }

        [XmlElement("TIPMOV")]
        public string CodigoTipoMovimentacao { get; set; }

        [XmlElement("DTNEG")]
        public string Data { get; set; }

        [XmlElement("CODTIPVENDA")]
        public string CodigoTipoVenda { get; set; }

        [XmlElement("CODPARC")]
        public string CodigoIntegracaoParceiro { get; set; }

        [XmlElement("CODTIPOPER")]
        public string CodigoTipoOperacao { get; set; }

        [XmlElement("CODEMP")]
        public string CodigoIntegracaoEmpresa { get; set; }

        [XmlElement("CODVEND")]
        public string CodigoIntegracaoVendedor { get; set; }

        [XmlElement("OBSERVACAO")]
        public string Observacao { get; set; }
    }

    public class IncluirNFNotaItem
    {
        [XmlElement("NUNOTA")]
        public string CodigoIntegracaoNotaFiscal { get; set; }

        [XmlElement("SEQUENCIA")]
        public string Sequencia { get; set; }

        [XmlElement("CODPROD")]
        public string CodigoIntegracaoProduto { get; set; }

        [XmlElement("CODVOL")]
        public string SiglaUnidadeMedida { get; set; }

        [XmlElement("CODLOCALORIG")]
        public string CodidoLocalOrigem { get; set; }

        [XmlElement("CONTROLE")]
        public string Controle { get; set; }

        [XmlElement("QTDNEG")]
        public string Quantidade { get; set; }
    }
}
