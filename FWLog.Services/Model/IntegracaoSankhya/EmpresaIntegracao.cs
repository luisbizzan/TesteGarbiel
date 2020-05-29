using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TSIEMP")]
    public class EmpresaIntegracao
    {
        [Display(Name= "TSIEMP.CODEMP")]
        [TabelaIntegracao(DisplayName = "TSIEMP.CODEMP")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [Display(Name = "TSIEMP.RAZAOSOCIAL")]
        [TabelaIntegracao(DisplayName = "TSIEMP.RAZAOSOCIAL")]
        [Required]
        public string RazaoSocial { get; set; }

        [Display(Name = "TSIEMP.NOMEFANTASIA")]
        [TabelaIntegracao(DisplayName = "TSIEMP.NOMEFANTASIA")]
        [Required]
        public string NomeFantasia { get; set; }

        [Display(Name = "TSIEMP.CGC")]
        [TabelaIntegracao(DisplayName = "TSIEMP.CGC")]
        public string CNPJ { get; set; }

        [Display(Name = "TSIEMP.CEP")]
        [TabelaIntegracao(DisplayName = "TSIEMP.CEP")]
        public string CEP { get; set; }

        [Display(Name = "TSIEND.NOMEEND")]
        [TabelaIntegracao(DisplayName = "CONCAT(CONCAT(TSIEND.TIPO, ' '), TSIEND.NOMEEND)")]
        public string Endereco { get; set; }

        [Display(Name = "TSIEMP.NUMEND")]
        [TabelaIntegracao(DisplayName = "TSIEMP.NUMEND")]
        public string Numero { get; set; }

        [Display(Name = "TSIEMP.COMPLEMENTO")]
        [TabelaIntegracao(DisplayName = "TSIEMP.COMPLEMENTO")]
        public string Complemento { get; set; }

        [Display(Name = "TSIBAI.NOMEBAI")]
        [TabelaIntegracao(DisplayName = "TSIBAI.NOMEBAI")]
        public string Bairro { get; set; }

        [Display(Name = "TSICID.NOMECID")]
        [TabelaIntegracao(DisplayName = "TSICID.NOMECID")]
        public string Cidade { get; set; }

        [Display(Name = "TSIEMP.TELEFONE")]
        [TabelaIntegracao(DisplayName = "TSIEMP.TELEFONE")]
        public string Telefone { get; set; }

        [Display(Name = "TGFEMP.ATIVO")]
        [TabelaIntegracao(DisplayName = "TGFEMP.ATIVO")]
        public string Ativo { get; set; }

        [Display(Name = "TSIEMP.CODEMPMATRIZ")]
        [TabelaIntegracao(DisplayName = "TSIEMP.CODEMPMATRIZ")]
        public string EmpresaMatriz { get; set; }

        [Display(Name = "TSIEMP.AD_FILIAL")]
        [TabelaIntegracao(DisplayName = "TSIEMP.AD_FILIAL")]
        [Required]
        public string Sigla { get; set; }

        [Display(Name = "TSIUFS.DESCRICAO")]
        [TabelaIntegracao(DisplayName = "TSIUFS.DESCRICAO")]
        public string Estado { get; set; }
        
        [Display(Name = "TGFEMP.AD_FONE_SAC")]
        [TabelaIntegracao(DisplayName = "TGFEMP.AD_FONE_SAC")]
        [Required]
        public string TelefoneSAC { get; set; }
    }
}
