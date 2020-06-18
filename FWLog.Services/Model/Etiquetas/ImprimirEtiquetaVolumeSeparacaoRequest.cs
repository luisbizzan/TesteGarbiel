using System;

namespace FWLog.Services.Model.Etiquetas
{
    public class ImprimirEtiquetaVolumeSeparacaoRequest
    {
        public string ClienteNomeFantasia { get; set; }
        public string ClienteEndereco { get; set; }
        public string ClienteEnderecoNumero { get; set; }
        public string ClienteCEP { get; set; }
        public string ClienteCidade { get; set; }
        public string ClienteUF { get; set; }
        public string ClienteTelefone { get; set; }
        public string ClienteCodigo { get; set; }
        public string PedidoCodigo { get; set; }
        public DateTime PedidoDataCriacao { get; set; }
        public bool PedidoIsRequisicao { get; set; }
        public int? PedidoPagamentoCodigoIntegracao { get; set; }
        public bool PedidoPagamentoIsDebito { get; set; }
        public bool PedidoPagamentoIsCredito { get; set; }
        public bool PedidoPagamentoIsDinheiro { get; set; }
        public string Centena { get; set; }
        public string TransportadoraSigla { get; set; }
        public string IdTransportadora { get; set; }
        public string TransportadoraNome { get; set; }
        public string CorredoresInicio { get; set; }
        public string CorredoresFim { get; set; }
        public string CaixaTextoEtiqueta { get; set; }
        public string Volume { get; set; }
        public long IdImpressora { get; set; }
        public string ProdutoReferencia { get; set; }
    }
}