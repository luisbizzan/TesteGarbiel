namespace FWLog.Data.Models.FilterCtx
{
    public class CorredorImpressoraListaFiltro
    {
        public long IdEmpresa { get; set; }
        public long? IdPontoArmazenagem { get; set; }
        public int? CorredorInicial { get; set; }
        public int? CorredorFinal { get; set; }
        public int? IdImpressora { get; set; }
        public bool? Status { get; set; }
    }
}
