using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TDDOPC")]
    public class FreteTipoIntegracao
    {
        [TabelaIntegracao(DisplayName = "VALOR")]
        [Required]
        public string Sigla { get; set; }

        [TabelaIntegracao(DisplayName = "OPCAO")]
        [Required]
        public string Descricao { get; set; }
    }
}
