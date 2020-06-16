using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioPedidosFiltro
    {
        public long? NumeroPedido { get; set; }
        public long? IdCliente { get; set; }
        public long IdEmpresa { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public long? IdTransportadora { get; set; }
        public long? IdStatus { get; set; }
        public long? IdProduto { get; set; }
    }
}
