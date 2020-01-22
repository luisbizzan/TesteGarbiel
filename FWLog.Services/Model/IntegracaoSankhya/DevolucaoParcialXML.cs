using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot(ElementName = "serviceRequest")]
    public class DevolucaoParcialXML
    {
        public DevolucaoParcialXML(string nota)
        {
            RequestBody = new RequestBody(nota);
        }

        [XmlAttribute(AttributeName = "serviceName")]
        public string ServiceName { get; } = "SelecaoDocumentoSP.faturar";

        [XmlElement(ElementName = "requestBody")]
        public RequestBody RequestBody { get; }
    }

    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlAttribute(AttributeName = "QTDFAT")]
        public string QTDFAT { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "itens")]
    public class Itens
    {
        [XmlElement(ElementName = "item")]
        public Item Item { get; set; }
    }

    [XmlRoot(ElementName = "nota")]
    public class Nota
    {
        [XmlElement(ElementName = "itens")]
        public Itens Itens { get; set; }

        [XmlAttribute(AttributeName = "NUNOTA")]
        public string NUNOTA { get; set; }
    }

    [XmlRoot(ElementName = "notas")]
    public class Notas2 : NotasBase
    {
        public Nota Nota { get; set; }
    }

    public abstract class NotasBase
    {
        #region Fixos

        [XmlElement(ElementName = "notasComMoeda")]
        public NotasComMoeda NotasComMoeda { get; } = new NotasComMoeda();

        [XmlAttribute(AttributeName = "codTipOper")]
        public string CodTipOper { get; } = "1301";

        [XmlAttribute(AttributeName = "dtFaturamento")]
        public string DtFaturamento { get; } = string.Empty;

        [XmlAttribute(AttributeName = "serie")]
        public string Serie { get; } = string.Empty;

        [XmlAttribute(AttributeName = "dtSaida")]
        public string DtSaida { get; } = string.Empty;

        [XmlAttribute(AttributeName = "hrSaida")]
        public string HrSaida { get; } = string.Empty;

        [XmlAttribute(AttributeName = "tipoFaturamento")]
        public string TipoFaturamento { get; } = "FaturamentoNormal";

        [XmlAttribute(AttributeName = "codLocalDestino")]
        public string CodLocalDestino { get; } = string.Empty;

        [XmlAttribute(AttributeName = "dataValidada")]
        public string DataValidada { get; } = true.ToString();

        [XmlAttribute(AttributeName = "faturarTodosItens")]
        public string FaturarTodosItens { get; } = true.ToString();

        [XmlAttribute(AttributeName = "umaNotaParaCada")]
        public string UmaNotaParaCada { get; } = false.ToString();

        [XmlAttribute(AttributeName = "ownerServiceCall")]
        public string OwnerServiceCall { get; } = "FaturamentoPopup788";

        [XmlAttribute(AttributeName = "ehWizardFaturamento")]
        public string EhWizardFaturamento { get; } = true.ToString();

        [XmlAttribute(AttributeName = "ehPedidoWeb")]
        public string EhPedidoWeb { get; } = false.ToString();

        [XmlAttribute(AttributeName = "nfeDevolucaoViaRecusa")]
        public string NfeDevolucaoViaRecusa { get; } = false.ToString();

        #endregion

    }
}
