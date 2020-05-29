using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class FornecedorIntegracao
    {
        [Display(Name= "TGFPAR.CODPARC")]
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

        [Display(Name = "TGFPAR.CEP")]
        [TabelaIntegracao(DisplayName = "TGFPAR.CEP")]
        public string CEP { get; set; }

        [Display(Name = "TSIEND.NOMEEND")]
        [TabelaIntegracao(DisplayName = "CONCAT(CONCAT(TSIEND.TIPO, ' '), TSIEND.NOMEEND)")]
        public string Endereco { get; set; }

        [Display(Name = "TGFPAR.NUMEND")]
        [TabelaIntegracao(DisplayName = "TGFPAR.NUMEND")]
        public string Numero { get; set; }

        [Display(Name = "TGFPAR.COMPLEMENTO")]
        [TabelaIntegracao(DisplayName = "TGFPAR.COMPLEMENTO")]
        public string Complemento { get; set; }

        [Display(Name = "TSIBAI.NOMEBAI")]
        [TabelaIntegracao(DisplayName = "TSIBAI.NOMEBAI")]
        public string Bairro { get; set; }

        [Display(Name = "TSICID.NOMECID")]
        [TabelaIntegracao(DisplayName = "TSICID.NOMECID")]
        public string Cidade { get; set; }

        [Display(Name = "TGFPAR.TELEFONE")]
        [TabelaIntegracao(DisplayName = "TGFPAR.TELEFONE")]
        public string Telefone { get; set; }

        [Display(Name = "TSIUFS.DESCRICAO")]
        [TabelaIntegracao(DisplayName = "TSIUFS.DESCRICAO")]
        public string Estado { get; set; }
    }
}
