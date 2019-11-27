using FWLog.Services.Integracao.Helpers;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TSIEMP")]
    public class EmpresaIntegracao
    {
        [TabelaIntegracao(DisplayName = "TSIEMP.CODEMP ")]
        public string CODEMP { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.RAZAOSOCIAL")]
        public string RAZAOSOCIAL { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.NOMEFANTASIA")]
        public string NOMEFANTASIA { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.CGC")]
        public string CGC { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.CEP")]
        public string CEP { get; set; }

        [TabelaIntegracao(DisplayName = "CONCAT(CONCAT(TSIEND.TIPO, ' '), TSIEND.NOMEEND)")]
        public string NOMEEND { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.NUMEND")]
        public string NUMEND { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.COMPLEMENTO")]
        public string COMPLEMENTO { get; set; }

        [TabelaIntegracao(DisplayName = "TSIBAI.NOMEBAI")]
        public string NOMEBAI { get; set; }

        [TabelaIntegracao(DisplayName = "TSICID.NOMECID")]
        public string NOMECID { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.TELEFONE")]
        public string TELEFONE { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.AD_ATIVOSAV")]
        public string AD_ATIVOSAV { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.CODEMPMATRIZ")]
        public string CODEMPMATRIZ { get; set; }

        [TabelaIntegracao(DisplayName = "TSIEMP.AD_UNIDABREV")]
        public string AD_UNIDABREV { get; set; }

        [TabelaIntegracao(DisplayName = "TSIUFS.DESCRICAO")]
        public string ESTADO { get; set; }
    }
}
