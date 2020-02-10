using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "AD_VGWMEDIA")]
    public class ProdutoMediaVendaIntegracao
    {
        [TabelaIntegracao(DisplayName = "CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [TabelaIntegracao(DisplayName = "CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [TabelaIntegracao(DisplayName = "MEDIA_6_MESES")]
        [Required]
        public float MediaVenda { get; set; }
    }
}
