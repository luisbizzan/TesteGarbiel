namespace FWLog.Data.Models.FilterCtx
{
    public class ProdutoPesquisaModalFiltro
    {
        public string Referencia { get; set; }
        public string Descricao { get; set; }
        public bool? Status { get; set; }
        public long? IdLote { get; set; }
        public long? IdPedidoVendaVolume { get; set; }
    }
}
