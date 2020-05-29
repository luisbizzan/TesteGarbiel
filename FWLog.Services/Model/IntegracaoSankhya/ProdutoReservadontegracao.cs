using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFEST")]
    public class ProdutoReservadoIntegracao
    {
        [Display(Name= "TGFEST.CODPROD")]
        [TabelaIntegracao(DisplayName = "CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [Display(Name = "TGFEST.CODEMP")]
        [TabelaIntegracao(DisplayName = "CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [Display(Name = "TGFEST.RESERVADO")]
        [TabelaIntegracao(DisplayName = "RESERVADO")]
        [Required]
        public string Reservado { get; set; }
    }
}
