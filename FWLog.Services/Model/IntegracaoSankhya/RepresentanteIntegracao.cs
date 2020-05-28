using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class RepresentanteIntegracao
    {
        [Display(Name = "TGFPAR.CODPARC")]
        [TabelaIntegracao(DisplayName = "TGFPAR.CODPARC")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [Display(Name = "GFPAR.NOMEPARC")]
        [TabelaIntegracao(DisplayName = "TGFPAR.NOMEPARC")]
        [Required]
        public string NomeFantasia { get; set; }

        [Display(Name = "TGFPAR.RAZAOSOCIAL")]
        [TabelaIntegracao(DisplayName = "TGFPAR.RAZAOSOCIAL")]
        [Required]
        public string RazaoSocial { get; set; }

        [Display(Name = "TGFPAR.CGC_CPF")]
        [TabelaIntegracao(DisplayName = "TGFPAR.CGC_CPF")]
        [Required]
        public string CNPJ { get; set; }

        [Display(Name = "TGFPAR.ATIVO")]
        [TabelaIntegracao(DisplayName = "TGFPAR.ATIVO")]
        [Required]
        public string Ativo { get; set; }

        [Display(Name = "TGFVEN.CODVEND")]
        [TabelaIntegracao(DisplayName = "TGFVEN.CODVEND")]
        [Required]
        public string CodigoIntegracaoVendedor { get; set; }
    }
}