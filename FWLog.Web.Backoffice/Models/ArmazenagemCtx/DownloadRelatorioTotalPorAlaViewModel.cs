namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class DownloadRelatorioTotalPorAlaViewModel
    {
        public long IdNivelArmazenagem { get; set; }
        public long IdPontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public bool ImprimirVazia { get; set; }
    }
}