using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioPedidosExpedidosLinhaTabela
    {
        public long NroPedido { get; set; }
        public DateTime? DataDoPedido { get; set; }
        public string NomeTransportadora { get; set; }
        public long IdPedidoVendaVolume { get; set; }
        public string NotaFiscalESerie { get; set; }
        public DateTime? DataSaidaDoPedido { get; set; }
    }
}
