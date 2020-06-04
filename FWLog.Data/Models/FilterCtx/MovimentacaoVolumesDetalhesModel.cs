using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class MovimentacaoVolumesDetalhesModel
    {
        public int PedidoNumero { get; set; }

        public int VolumeNumero { get; set; }

        public int QuantidadeProdutos { get; set; }

        public DateTime PedidoData { get; set; }

        public long IdPedidoVendaVolume { get; set; }
    }
}