using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class RepresentanteIntegracao
    {
        [TabelaIntegracao(DisplayName = "TGFPAR.CODPARC")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.NOMEPARC")]
        [Required]
        public string NomeFantasia { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.RAZAOSOCIAL")]
        [Required]
        public string RazaoSocial { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.CGC_CPF")]
        [Required]
        public string CNPJ { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.ATIVO")]
        [Required]
        public string Ativo { get; set; }

        [TabelaIntegracao(DisplayName = "TGFVEN.CODVEND")]
        [Required]
        public string CodigoIntegracaoVendedor { get; set; }
    }
}
