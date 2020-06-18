﻿using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class MovimentacaoVolumesDetalhesModel
    {
        public long IdPedidoVendaVolume { get; set; }

        public long IdPedidoVenda { get; set; }

        public string PedidoNumero { get; set; }

        public int VolumeNumero { get; set; }

        public int QuantidadeProdutos { get; set; }

        public DateTime PedidoDataCriacao { get; set; }

        public DateTime PedidoDataIntegracao { get; set; }

        public int VolumeCentena { get; set; }

        public string TransportadoraNomeFantasia { get; set; }

        public string TipoPagamentoDescricao { get; set; }

        public string UsuarioDespachoNotaFiscal { get; set; }

        public DateTime? DataHoraDespachoNotaFiscal { get; set; }

        public int? NumeroRomaneio { get; set; }

        public string UsuarioRomaneio { get; set; }

        public DateTime? DataHoraRomaneio { get; set; }
    }
}