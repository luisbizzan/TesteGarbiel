namespace FWLog.Data.Models.FilterCtx
{
    public class PedidoVendaFiltro
    {
        public long IdEmpresa { get; set; }

        public int? NumeroPedido { get; set; }

        public int? NumeroPedidoVenda { get; set; }

        public string NomeTransportadora { get; set; }
    }
}