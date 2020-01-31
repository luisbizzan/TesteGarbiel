using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot(ElementName = "serviceRequest")]
    public class DevolucaoTotalXML
    {
        public DevolucaoTotalXML() { }
        public DevolucaoTotalXML(string nota, string codTipOper)
        {
            RequestBody = new ElementoCorpoDevolucao(nota, codTipOper);
        }

        [XmlAttribute(AttributeName = "serviceName")]
        public string ServiceName { get; set; } = "SelecaoDocumentoSP.faturar";

        [XmlElement(ElementName = "requestBody")]
        public ElementoCorpoDevolucao RequestBody { get; set; }
    }

    public class ElementoCorpoDevolucao
    {
        public ElementoCorpoDevolucao() { }
        public ElementoCorpoDevolucao(string codigoIntegracao, string codTipOper)
        {
            Notas = new ElementoNotas(codigoIntegracao, codTipOper);
        }

        [XmlElement(ElementName = "notas")]
        public ElementoNotas Notas { get; set; }
    }

    public class NotasComMoeda
    {
        public NotasComMoeda() { }
        [XmlAttribute(AttributeName = "valorMoeda")]
        public string ValorMoeda { get; set; } = "undefined";
    }

    public class ElementoNotas : ElementoNotasBase
    {
        public ElementoNotas() { }

        public ElementoNotas(string codigoIntegracao, string codTipOper)
        {
            Nota = codigoIntegracao;
            CodTipOper = codTipOper;
        }

        [XmlElement(ElementName = "nota")]
        public string Nota { get; set; }
    }
}
