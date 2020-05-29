using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPEM")]
    public class ProdutoEstoqueIntegracao
    {
        [Display(Name = "TGFPEM.CODPROD")]
        [TabelaIntegracao(DisplayName = "CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [Display(Name = "TGFPEM.CODEMP")]
        [TabelaIntegracao(DisplayName = "CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [Display(Name = "TGFPEM.AD_STATUS")]
        [TabelaIntegracao(DisplayName = "AD_STATUS")]
        [Required]
        public string Status { get; set; }

        [Display(Name = "TGFPEM.LEADTIME")]
        [TabelaIntegracao(DisplayName = "LEADTIME")]
        [Required]
        public string DiasPrazoEntrega { get; set; }
    }
}
