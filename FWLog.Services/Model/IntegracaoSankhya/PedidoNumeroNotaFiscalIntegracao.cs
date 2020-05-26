using FWLog.Services.Integracao.Helpers;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [TabelaIntegracao(DisplayName = "TGFCAB")]
    public class PedidoNumeroNotaFiscalIntegracao
    {
        [TabelaIntegracao(DisplayName = "TGFVAR.NUNOTAORIG")]
        public string CodigoIntegracao { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.NUNOTA")]
        public string CodigoIntegracaoNotaFiscal { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.NUMNOTA")]
        public string NumeroNotaFiscal { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.SERIENOTA")]
        public string SerieNotaFiscal { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.CHAVENFE")]
        public string ChaveAcesso { get; set; }

        [TabelaIntegracao(DisplayName = "TGFCAB.CIF_FOB")]
        public string TipoFrete { get; set; }
    }
}