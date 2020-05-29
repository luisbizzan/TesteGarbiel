using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class TransportadoraIntegracao
    {
        [Display(Name = "TGFPAR.CODPARC")]
        [TabelaIntegracao(DisplayName = "CODPARC")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [Display(Name = "TGFPAR.NOMEPARC")]
        [TabelaIntegracao(DisplayName = "NOMEPARC")]
        [Required]
        public string NomeFantasia { get; set; }

        [Display(Name = "TGFPAR.RAZAOSOCIAL")]
        [TabelaIntegracao(DisplayName = "RAZAOSOCIAL")]
        [Required]
        public string RazaoSocial { get; set; }

        [Display(Name = "TGFPAR.CGC_CPF")]
        [TabelaIntegracao(DisplayName = "CGC_CPF")]
        [Required]
        public string CNPJ { get; set; }

        [Display(Name = "TGFPAR.ATIVO")]
        [TabelaIntegracao(DisplayName = "ATIVO")]
        [Required]
        public string Ativo { get; set; }

        [Display(Name = "TGFPAR.AD_ABREVTRANSP")]
        [TabelaIntegracao(DisplayName = "AD_ABREVTRANSP")]
        [Required]
        public string CodigoTransportadora { get; set; }
    }
}