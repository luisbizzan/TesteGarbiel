using FWLog.Services.Integracao.Helpers;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFPAR")]
    public class TransportadoraIntegracao
    {
        public string CODPARC { get; set; }
        public string NOMEPARC { get; set; }
        public string RAZAOSOCIAL { get; set; }
        public string CGC_CPF { get; set; }
        public string ATIVO { get; set; }
    }
}
