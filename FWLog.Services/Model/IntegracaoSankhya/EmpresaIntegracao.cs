using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TSIEMP")]
    public class EmpresaIntegracao
    {
        [TabelaIntegracao(DisplayName = "TSIEMP.CODEMP ")]
        [Required]
        public string CodigoIntegracao { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.RAZAOSOCIAL")]
        public string RazaoSocial { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.NOMEFANTASIA")]
        [Required]
        public string NomeFantasia { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.CGC")]
        public string CNPJ { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.CEP")]
        public string CEP { get; set; }

        [TabelaIntegracao(DisplayName = "CONCAT(CONCAT(TSIEND.TIPO, ' '), TSIEND.NOMEEND)")]
        public string Endereco { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.NUMEND")]
        public string Numero { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.COMPLEMENTO")]
        public string Complemento { get; set; }

        [TabelaIntegracao(DisplayName = "TSIBAI.NOMEBAI")]
        public string Bairro { get; set; }

        [TabelaIntegracao(DisplayName = "TSICID.NOMECID")]
        public string Cidade { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.TELEFONE")]
        public string Telefone { get; set; }

        [TabelaIntegracao(DisplayName = "TGFEMP.ATIVO")]
        public string Ativo { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.CODEMPMATRIZ")]
        public string EmpresaMatriz { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.AD_FILIAL")]
        [Required]
        public string Sigla { get; set; }

        [TabelaIntegracao(DisplayName = "TSIUFS.DESCRICAO")]
        public string Estado { get; set; }

        [TabelaIntegracao(DisplayName = "TGFEMP.AD_FONE_SAC")]
        public string TelefoneSAC { get; set; }
    }
}
