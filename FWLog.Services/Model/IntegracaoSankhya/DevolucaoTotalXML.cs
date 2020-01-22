using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{

    [XmlRoot(ElementName = "serviceRequest")]
    public class DevolucaoTotalXML
    {
        public DevolucaoTotalXML(string nota)
        {
            RequestBody = new RequestBody(nota);
        }

        [XmlAttribute(AttributeName = "serviceName")]
        public string ServiceName { get; } = "SelecaoDocumentoSP.faturar";

        [XmlElement(ElementName = "requestBody")]
        public RequestBody RequestBody { get; }
    }

    [XmlRoot(ElementName = "requestBody")]
    public class RequestBody
    {
        public RequestBody(string nota)
        {
            Notas = new Notas1(nota);
        }

        [XmlElement(ElementName = "notas")]
        public Notas1 Notas { get; }
    }


    [XmlRoot(ElementName = "notas")]
    public class Notas1
    {
        public Notas1(string nota)
        {
            Nota = nota;
        }

        [XmlElement(ElementName = "nota")]
        public string Nota { get; }

    }

    [XmlRoot(ElementName = "notasComMoeda")]
    public class NotasComMoeda
    {
        [XmlAttribute(AttributeName = "valorMoeda")]
        public string ValorMoeda { get; }
    }


}
