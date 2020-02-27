using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class ClienteIntegracao
    {
        [TabelaIntegracao(DisplayName = "CODPARC")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [TabelaIntegracao(DisplayName = "NOMEPARC")]
        [Required]
        public string NomeFantasia { get; set; }

        [TabelaIntegracao(DisplayName = "RAZAOSOCIAL")]
        [Required]
        public string RazaoSocial { get; set; }

        [TabelaIntegracao(DisplayName = "CGC_CPF")]
        [Required]
        public string CNPJ { get; set; }

        [TabelaIntegracao(DisplayName = "ATIVO")]
        [Required]
        public string Ativo { get; set; }

        //[TabelaIntegracao(DisplayName = "CODVEND")]
        //[Required]
        //public long IdRepresentante { get; set; }

        //[TabelaIntegracao(DisplayName = "AD_CLASSIF")]
        //[Required]
        //public string Classificacao { get; set; }
    }
}
