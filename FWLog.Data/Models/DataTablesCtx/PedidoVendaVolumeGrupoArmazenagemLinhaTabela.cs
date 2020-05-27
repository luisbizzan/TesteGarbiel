namespace FWLog.Data.Models.DataTablesCtx
{
    public class PedidoVendaVolumeGrupoArmazenagemLinhaTabela
    {
        public long IdGrupoCorredorArmazenagem { get; set; }

        public string PontoArmazenagemDescricao { get; set; }

        public int CorredorInicial { get; set; }

        public int CorredorFinal { get; set; }

        public long IdPedidoVendaVolume { get; set; }

        public PedidoVendaStatusEnum IdPedidoVendaStatus { get; set; }
    }
}