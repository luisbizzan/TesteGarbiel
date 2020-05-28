﻿using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioPedidosLinhaTabela
    {
        public string NroPedido { get; set; }
        public long IdPedidoVendaVolume { get; set; }
        public string NomeTransportadora { get; set; }
        public string Status { get; set; }
        public string NroVolume { get; set; }
        public DateTime DataDoPedido { get; set; }
        public DateTime? DataSaidaDoPedido { get; set; }
    }
}
