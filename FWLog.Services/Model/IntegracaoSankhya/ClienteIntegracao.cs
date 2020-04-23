using FWLog.Services.Integracao.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class ClienteIntegracao
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

        [TabelaIntegracao(DisplayName = "TGFPAR.AD_CODVENDATEND")]
        public string IdRepresentanteInterno { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.CODVEND")]
        public string IdRepresentanteExterno { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.AD_CLASSIF")]
        [Required]
        public string Classificacao { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCPL.CEPENTREGA")]
        public string CEP { get; set; }

        [TabelaIntegracao(DisplayName = "CONCAT(CONCAT(TSIEND.TIPO, ' '), TSIEND.NOMEEND)")]
        public string Endereco { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCPL.NUMENTREGA")]
        public string Numero { get; set; }

        [TabelaIntegracao(DisplayName = "TGFPAR.TELEFONE")]
        public string Telefone { get; set; }

        [TabelaIntegracao(DisplayName = "TSIUFS.UF")]
        public string UF { get; set; }

        [TabelaIntegracao(DisplayName = "TSICID.NOMECID")]
        public string Cidade { get; set; }
    }
}
