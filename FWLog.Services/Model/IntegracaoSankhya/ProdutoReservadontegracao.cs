using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFEST")]
    public class ProdutoReservadoIntegracao
    {
        [TabelaIntegracao(DisplayName = "CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [TabelaIntegracao(DisplayName = "CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [TabelaIntegracao(DisplayName = "RESERVADO")]
        [Required]
        public string Reservado { get; set; }
    }
}
