using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class TransportadoraIntegracao
    {
        [Display(Name = "CODPARC")]
        [TabelaIntegracao(DisplayName = "CODPARC")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [Display(Name = "NOMEPARC")]
        [TabelaIntegracao(DisplayName = "NOMEPARC")]
        [Required]
        public string NomeFantasia { get; set; }

        [Display(Name = "RAZAOSOCIAL")]
        [TabelaIntegracao(DisplayName = "RAZAOSOCIAL")]
        [Required]
        public string RazaoSocial { get; set; }

        [Display(Name = "CGC_CPF")]
        [TabelaIntegracao(DisplayName = "CGC_CPF")]
        [Required]
        public string CNPJ { get; set; }

        [Display(Name = "ATIVO")]
        [TabelaIntegracao(DisplayName = "ATIVO")]
        [Required]
        public string Ativo { get; set; }

        [Display(Name = "AD_ABREVTRANSP")]
        [TabelaIntegracao(DisplayName = "AD_ABREVTRANSP")]
        [Required]
        public string CodigoTransportadora { get; set; }
    }
}