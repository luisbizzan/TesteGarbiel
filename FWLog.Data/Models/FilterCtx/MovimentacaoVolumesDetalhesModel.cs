using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class MovimentacaoVolumesDetalhesModel
    {
        public long IdPedidoVendaVolume { get; set; }

        public int PedidoNumero { get; set; }

        public int VolumeNumero { get; set; }

        public int QuantidadeProdutos { get; set; }

        public DateTime PedidoData { get; set; }

        public int VolumeCentena { get; set; }

        public string TransportadoraNomeFantasia { get; set; }

        public string TipoPagamentoDescricao { get; set; }
    }
}