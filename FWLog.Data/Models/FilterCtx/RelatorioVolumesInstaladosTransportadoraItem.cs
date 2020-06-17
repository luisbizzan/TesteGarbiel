namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioVolumesInstaladosTransportadoraItem
    {
        public long IdTransportadora { get; set; }

        public string TransportadoraNome { get; set; }

        public string CodigoEndereco { get; set; }

        public string NumeroPedido { get; set; }

        public int NumeroVolume { get; set; }

        public string StatusVolume { get; set; }
    }
}