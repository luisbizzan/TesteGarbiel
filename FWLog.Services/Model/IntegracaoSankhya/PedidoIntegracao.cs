using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class PedidoIntegracao
    {
        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.NUMNOTA")]
        [Required]
        public string NroPedidoVenda { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARC")]
        [Required]
        public string CodigoIntegracaoCliente { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARCTRANSP")]
        [Required]
        public string CodigoIntegracaoTransportadora { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.DTNEG")]
        public string DataCriacao { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.CODVEND")]
        [Required]
        public string CodigoIntegracaoRepresentante { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.QTDNEG")]
        [Required]
        public string QtdPedido { get; set; }

        [TabelaIntegracao(DisplayName = "TGFITE.SEQUENCIA")]
        [Required]
        public string Sequencia { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.CODTIPOPER")]
        [Required]
        public string TopSankhya { get; set; }

        [TabelaIntegracao(DisplayName = "TGFTPV.CODTIPVENDA")]
        public string TipoPagamentoCodigo { get; set; }

        [TabelaIntegracao(DisplayName = "TGFTPV.DESCRTIPVENDA")]
        public string TipoPagamentoDescricao { get; set; }

        [TabelaIntegracao(DisplayName = "TGFTPV.AD_CARTAODEBITO")]
        public string TipoPagamentoCartaoDebito { get; set; }

        [TabelaIntegracao(DisplayName = "TGFTPV.AD_CARTAOCREDITO")]
        public string TipoPagamentoCartaoCredito { get; set; }
    }
}