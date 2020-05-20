namespace FWLog.Data.Models.FilterCtx
{
    public class PedidoVendaItem
    {
        public long IdPedidoVenda { get; set; }

        public int NumeroPedidoVenda { get; set; }

        public int NumeroPedido { get; set; }

        public string ClienteNome { get; set; }

        public string TransportadoraNome { get; set; }
    }
}