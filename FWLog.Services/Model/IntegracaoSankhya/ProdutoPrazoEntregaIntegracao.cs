using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPEM")]
    public class ProdutoPrazoEntregaIntegracao
    {
        [TabelaIntegracao(DisplayName = "CODPROD")]
        [Required]
        public string CodigoIntegracaoProduto { get; set; }

        [TabelaIntegracao(DisplayName = "CODEMP")]
        [Required]
        public string CodigoIntegracaoEmpresa { get; set; }

        [TabelaIntegracao(DisplayName = "LEADTIME")]
        [Required]
        public string DiasPrazoEntrega { get; set; }
    }
}

