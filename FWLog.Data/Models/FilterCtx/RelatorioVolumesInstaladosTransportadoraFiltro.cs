namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioVolumesInstaladosTransportadoraFiltro
    {
        public long IdEmpresa { get; set; }

        public long? IdTransportadora { get; set; }

        public string EnderecoCodigo { get; set; }

        public long? IdPedidoVenda { get; set; }
    }
}