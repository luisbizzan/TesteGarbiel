using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TDDOPC")]
    public class FreteTipoIntegracao
    {
        [Display(Name = "TDDOPC.VALOR")]
        [TabelaIntegracao(DisplayName = "VALOR")]
        [Required]
        public string Sigla { get; set; }

        [Display(Name = "TDDOPC.OPCAO")]
        [TabelaIntegracao(DisplayName = "OPCAO")]
        [Required]
        public string Descricao { get; set; }
    }
}
