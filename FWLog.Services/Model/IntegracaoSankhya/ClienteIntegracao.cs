using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class ClienteIntegracao
    {
        [Display(Name = "TGFPAR.CODPARC")]
        [TabelaIntegracao(DisplayName = "TGFPAR.CODPARC")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [Display(Name = "TGFPAR.NOMEPARC")]
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

        [Display(Name = "TGFPAR.AD_CODVENDATEND")]
        [TabelaIntegracao(DisplayName = "TGFPAR.AD_CODVENDATEND")]
        public string IdRepresentanteInterno { get; set; }

        [Display(Name = "TGFPAR.CODVEND")]
        [TabelaIntegracao(DisplayName = "TGFPAR.CODVEND")]
        public string IdRepresentanteExterno { get; set; }

        [Display(Name = "TGFPAR.AD_CLASSIF")]
        [TabelaIntegracao(DisplayName = "TGFPAR.AD_CLASSIF")]
        public string Classificacao { get; set; }

        [Display(Name = "TGFCPL.CEPENTREGA")]
        [TabelaIntegracao(DisplayName = "TGFCPL.CEPENTREGA")]
        [Required]
        public string CEP { get; set; }

        [Display(Name = "TSIEND.NOMEEND")]
        [TabelaIntegracao(DisplayName = "CONCAT(CONCAT(TSIEND.TIPO, ' '), TSIEND.NOMEEND)")]
        [Required]
        public string Endereco { get; set; }

        [Display(Name = "TGFCPL.NUMENTREGA")]
        [TabelaIntegracao(DisplayName = "TGFCPL.NUMENTREGA")]
        [Required]
        public string Numero { get; set; }

        [Display(Name = "TGFPAR.TELEFONE")]
        [TabelaIntegracao(DisplayName = "TGFPAR.TELEFONE")]
        [Required]
        public string Telefone { get; set; }

        [Display(Name = "TSIUFS.UF")]
        [TabelaIntegracao(DisplayName = "TSIUFS.UF")]
        [Required]
        public string UF { get; set; }

        [Display(Name = "TSICID.NOMECID")]
        [TabelaIntegracao(DisplayName = "TSICID.NOMECID")]
        [Required]
        public string Cidade { get; set; }

        [Display(Name = "CODENDENTREGA")]
        [TabelaIntegracao(DisplayName = "CODENDENTREGA")]
        public string CodigoIntgracaoEndereco { get; set; }

        [Display(Name = "CODCIDENTREGA")]
        [TabelaIntegracao(DisplayName = "CODCIDENTREGA")]
        public string CodigoIntegracaoCidade { get; set; }
    }
}
