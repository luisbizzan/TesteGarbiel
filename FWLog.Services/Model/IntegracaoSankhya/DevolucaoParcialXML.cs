using System.Collections.Generic;
using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot(ElementName = "serviceRequest")]
    public class DevolucaoParcialXML
    {
        public DevolucaoParcialXML() { }
        public DevolucaoParcialXML(long codigoIntegracao, string codTipOper, List<ElementoItemDetalhes> listItemDetalhes)
        {
            RequestBody = new ElementoCorpoDevolucaoParcial(codigoIntegracao, codTipOper, listItemDetalhes);
        }

        [XmlAttribute(AttributeName = "serviceName")]
        public string ServiceName { get; set; } = "SelecaoDocumentoSP.faturar";

        [XmlElement(ElementName = "requestBody")]
        public ElementoCorpoDevolucaoParcial RequestBody { get; set; }
    }

    public class ElementoCorpoDevolucaoParcial
    {
        public ElementoCorpoDevolucaoParcial() { }
        public ElementoCorpoDevolucaoParcial(long codigoIntegracao, string codTipOper, List<ElementoItemDetalhes> listItemDetalhes)
        {
            Notas = new ElementoNotasDevolucaoParcial(codigoIntegracao, codTipOper, listItemDetalhes);
        }

        [XmlElement(ElementName = "notas")]
        public ElementoNotasDevolucaoParcial Notas { get; set; }
    }
    public class ElementoNotasDevolucaoParcial : ElementoNotasBase
    {
        public ElementoNotasDevolucaoParcial() { }

        public ElementoNotasDevolucaoParcial(long codigoIntegracao, string codTipOper, List<ElementoItemDetalhes> listItemDetalhes)
        {
            Nota = new ElementoNotaDevolucaoParcial(codigoIntegracao, listItemDetalhes);
            CodTipOper = codTipOper;
        }

        [XmlElement(ElementName = "nota")]
        public ElementoNotaDevolucaoParcial Nota { get; set; }
    }

    public class ElementoNotasBase
    {
        public ElementoNotasBase() { }

        [XmlElement(ElementName = "notasComMoeda")]
        public NotasComMoeda NotasComMoeda { get; set; } = new NotasComMoeda();

        [XmlAttribute(AttributeName = "codTipOper")]
        public string CodTipOper { get; set; }

        [XmlAttribute(AttributeName = "dtFaturamento")]
        public string DtFaturamento { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "serie")]
        public string Serie { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "dtSaida")]
        public string DtSaida { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "hrSaida")]
        public string HrSaida { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "tipoFaturamento")]
        public string TipoFaturamento { get; set; } = "FaturamentoNormal";

        [XmlAttribute(AttributeName = "codLocalDestino")]
        public string CodLocalDestino { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "dataValidada")]
        public string DataValidada { get; set; } = true.ToString();

        [XmlAttribute(AttributeName = "faturarTodosItens")]
        public string FaturarTodosItens { get; set; } = true.ToString();

        [XmlAttribute(AttributeName = "umaNotaParaCada")]
        public string UmaNotaParaCada { get; set; } = false.ToString();

        [XmlAttribute(AttributeName = "ownerServiceCall")]
        public string OwnerServiceCall { get; set; } = "FaturamentoPopup788";

        [XmlAttribute(AttributeName = "ehWizardFaturamento")]
        public string EhWizardFaturamento { get; set; } = true.ToString();

        [XmlAttribute(AttributeName = "ehPedidoWeb")]
        public string EhPedidoWeb { get; set; } = false.ToString();

        [XmlAttribute(AttributeName = "nfeDevolucaoViaRecusa")]
        public string NfeDevolucaoViaRecusa { get; set; } = false.ToString();
    }

    public class ElementoItemDetalhes
    {
        [XmlAttribute(AttributeName = "QTDFAT")]
        public int Quantidade { get; set; }

        [XmlText]
        public string Sequencia { get; set; }

        [XmlIgnore]
        public long IdProduto { get; set; }
    }

    public class ElementoItem
    {
        public ElementoItem() { }
        public ElementoItem(List<ElementoItemDetalhes> listItemDetalhes)
        {
            ItemDetalhes = listItemDetalhes;
        }

        [XmlElement(ElementName = "item")]
        public List<ElementoItemDetalhes> ItemDetalhes { get; set; }
    }

    public class ElementoNotaDevolucaoParcial
    {
        public ElementoNotaDevolucaoParcial() { }

        public ElementoNotaDevolucaoParcial(long codigoIntegracao, List<ElementoItemDetalhes> listItemDetalhes)
        {
            CodigoIntegracao = codigoIntegracao.ToString();
            Itens = new ElementoItem(listItemDetalhes);
        }

        [XmlElement(ElementName = "itens")]
        public ElementoItem Itens { get; set; }

        [XmlAttribute(AttributeName = "NUNOTA")]
        public string CodigoIntegracao { get; set; }
    }
}
