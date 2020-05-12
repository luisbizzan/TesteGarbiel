namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaVolumeSeparacaoRequest
    {
        public string ClienteNome { get; set; }
        public string ClienteEndereco { get; set; }
        public string ClienteEnderecoNumero { get; set; }
        public string ClienteCEP { get; set; }
        public string ClienteCidade { get; set; }
        public string ClienteUF { get; set; }
        public string ClienteTelefone { get; set; }
        public string ClienteCodigo { get; set; }
        public string RepresentanteCodigo { get; set; }
        public string PedidoCodigo { get; set; }
        public string Centena { get; set; }
        public string TransportadoraSigla { get; set; }
        public string TransportadoraCodigo { get; set; }
        public string TransportadoraNome { get; set; }
        public string CorredoresInicio { get; set; }
        public string CorredoresFim { get; set; }
        public string CorredorInicioSeparacao { get; set; }
        public string CaixaTextoEtiqueta { get; set; }
        public string Volume { get; set; }
        public long IdImpressora { get; set; }
    }
}