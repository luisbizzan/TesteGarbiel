using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class PedidoIntegracaoPendente
    {
        [Display(Name = "TGFCAB.NUNOTA")]
        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        [Required]
        public string CodigoIntegracao { get; set; }
    }
}