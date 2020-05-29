using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFVOL")]
    public class UnidadeMedidaIntegracao
    {
        [Display(Name = "TGFVOL.CODVOL")]
        [TabelaIntegracao(DisplayName = "CODVOL")]
        [Required]
        public string Sigla { get; set; }

        [Display(Name = "TGFVOL.DESCRVOL")]
        [TabelaIntegracao(DisplayName = "DESCRVOL")]
        [Required]
        public string Descricao { get; set; }
    }
}
