using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class PedidoIntegracao
    {
        [Display(Name = "TGFCAB.NUNOTA")]
        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [Display(Name = "TGFCAB.NUMNOTA")]
        [TabelaIntegracao(DisplayName = "TGFCAB.NUMNOTA")]
        [Required]
        public string NroPedidoVenda { get; set; }

        [Display(Name = "TGFCAB.CODEMP")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [Display(Name = "TGFCAB.CODPARC")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARC")]
        [Required]
        public string CodigoIntegracaoCliente { get; set; }

        [Display(Name = "TGFCAB.CODPARCTRANSP")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODPARCTRANSP")]
        [Required]
        public string CodigoIntegracaoTransportadora { get; set; }

        [Display(Name = "TGFCAB.DTNEG")]
        [TabelaIntegracao(DisplayName = "TGFCAB.DTNEG")]
        [Required]
        public string DataCriacao { get; set; }

        [Display(Name = "TGFCAB.CODVEND")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODVEND")]
        [Required]
        public string CodigoIntegracaoRepresentante { get; set; }

        [Display(Name = "TGFITE.CODPROD")]
        [TabelaIntegracao(DisplayName = "TGFITE.CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [Display(Name = "TGFITE.QTDNEG")]
        [TabelaIntegracao(DisplayName = "TGFITE.QTDNEG")]
        [Required]
        public string QtdPedido { get; set; }

        [Display(Name = "TGFITE.SEQUENCIA")]
        [TabelaIntegracao(DisplayName = "TGFITE.SEQUENCIA")]
        [Required]
        public string Sequencia { get; set; }

        [Display(Name = "TGFCAB.CODTIPOPER")]
        [TabelaIntegracao(DisplayName = "TGFCAB.CODTIPOPER")]
        [Required]
        public string TopSankhya { get; set; }

        [Display(Name = "TGFTPV.CODTIPVENDA")]
        [TabelaIntegracao(DisplayName = "TGFTPV.CODTIPVENDA")]
        public string TipoPagamentoCodigo { get; set; }

        [Display(Name = "TGFTPV.DESCRTIPVENDA")]
        [TabelaIntegracao(DisplayName = "TGFTPV.DESCRTIPVENDA")]
        public string TipoPagamentoDescricao { get; set; }

        [Display(Name = "TGFTPV.AD_CARTAODEBITO")]
        [TabelaIntegracao(DisplayName = "TGFTPV.AD_CARTAODEBITO")]
        public string TipoPagamentoCartaoDebito { get; set; }

        [Display(Name = "TGFTPV.AD_CARTAOCREDITO")]
        [TabelaIntegracao(DisplayName = "TGFTPV.AD_CARTAOCREDITO")]
        public string TipoPagamentoCartaoCredito { get; set; }

        [Display(Name = "TGFTPV.AD_DINHEIRO")]
        [TabelaIntegracao(DisplayName = "TGFTPV.AD_DINHEIRO")]
        public string TipoPagamentoCartaoDinheiro { get; set; }
    }
}