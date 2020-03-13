using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class FornecedorIntegracao
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





        [TabelaIntegracao(DisplayName = "TGFPAR.CEP")]
        public string CEP { get; set; }

        [TabelaIntegracao(DisplayName = "CONCAT(CONCAT(TSIEND.TIPO, ' '), TSIEND.NOMEEND)")]
        public string Endereco { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.NUMEND")]
        public string Numero { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.COMPLEMENTO")]
        public string Complemento { get; set; }

        [TabelaIntegracao(DisplayName = "TSIBAI.NOMEBAI")]
        public string Bairro { get; set; }

        [TabelaIntegracao(DisplayName = "TSICID.NOMECID")]
        public string Cidade { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.TELEFONE")]
        public string Telefone { get; set; }


        [TabelaIntegracao(DisplayName = "TSIUFS.DESCRICAO")]
        public string Estado { get; set; }





    }
}
