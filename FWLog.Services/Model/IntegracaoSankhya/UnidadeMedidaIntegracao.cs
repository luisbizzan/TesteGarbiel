using FWLog.Services.Integracao.Helpers;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFVOL")]
    public class UnidadeMedidaIntegracao
    {
        public string CODVOL { get; set; }
        public string DESCRVOL { get; set; }
    }
}
