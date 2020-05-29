using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "AD_MEDVENDPRO")]
    public class ProdutoMediaVendaIntegracao
    {
        [Display(Name = "AD_MEDVENDPRO.CODPROD")]
        [TabelaIntegracao(DisplayName = "CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [Display(Name = "AD_MEDVENDPRO.CODEMP")]
        [TabelaIntegracao(DisplayName = "CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [Display(Name = "AD_MEDVENDPRO.MEDIA_6_MESES")]
        [TabelaIntegracao(DisplayName = "MEDIA_6_MESES")]
        [Required]
        public string MediaVenda { get; set; }
    }
}
