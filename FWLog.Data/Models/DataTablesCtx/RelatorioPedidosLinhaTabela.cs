namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioPedidosLinhaTabela
    {
        public string NroPedido { get; set; }
        public long IdPedidoVendaVolume { get; set; }
        public string NomeTransportadora { get; set; }
        public string Status { get; set; }
        public string NroVolume { get; set; }
        public string DataCriacao { get; set; }
        public string NumeroSerieNotaFiscal { get; set; }
        public string DataExpedicao { get; set; }
    }
}