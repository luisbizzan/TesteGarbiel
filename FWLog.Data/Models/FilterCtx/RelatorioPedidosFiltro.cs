using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioPedidosFiltro
    {
        public long? IdPedidoVenda { get; set; }
        public long? IdPedidoVendaVolume { get; set; }
        public long IdEmpresa { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public long? IdTransportadora { get; set; }
    }
}
