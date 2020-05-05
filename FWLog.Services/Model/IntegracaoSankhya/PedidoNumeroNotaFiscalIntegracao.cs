using FWLog.Services.Integracao.Helpers;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class PedidoNumeroNotaFiscalIntegracao
    {
        [TabelaIntegracao(DisplayName = "TGFVAR.NUNOTAORIG")]
        public string CodigoIntegracao { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        public string NumeroNotaFiscal { get; set; }
    }
}