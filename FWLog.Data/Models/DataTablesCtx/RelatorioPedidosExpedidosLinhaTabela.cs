using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioPedidosExpedidosLinhaTabela
    {
        public long NroPedido { get; set; }
        public string DataDoPedido { get; set; }
        public string IdENomeTransportadora { get; set; }
        public string NroVolume { get; set; }
        public string NroCentena { get; set; }
        public string NotaFiscalESerie { get; set; }
        public string DataSaidaDoPedido { get; set; }
    }
}
