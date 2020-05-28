using System;
using System.Collections.Generic;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class DetalhesPedidoVolume
    {
        public int PedidoNumero { get; set; }

        //TODO: Dados pedido

        public int VolumeNroVolume { get; set; }

        public int VolumeNroCentena { get; set; }

        public string VolumeCaixaCubagem { get; set; }

        public decimal VolumeCubagemVolume { get; set; }

        public decimal VolumePesoVolume { get; set; }

        public int VolumeCorredorInicio { get; set; }

        public int VolumeCorredorFim { get; set; }

        public string VolumeCaixaVolume { get; set; }

        public string VolumePedidoVendaStatus { get; set; }

        public string VolumeUsuarioInstalTransportadora { get; set; }

        public DateTime? VolumeDataHoraInstalTransportadora { get; set; }

        public string VolumeEnderecoArmazTransportadora { get; set; }

        public string VolumeUsuarioInstalacaoDOCA { get; set; }

        public DateTime? VolumeDataHoraInstalacaoDOCA { get; set; }

        public DateTime? VolumeDataHoraRemocaoVolume { get; set; }

        public List<DetalhesPedidoProdutoVolume> ListaProdutos { get; set; }
    }

    public class DetalhesPedidoProdutoVolume
    {
        public string ProdutoReferencia { get; set; }

        public int QuantidadeSeparar { get; set; }

        public int QuantidadeSeparada { get; set; }

        public DateTime? DataHoraInicioSeparacao { get; set; }

        public DateTime? DataHoraFimSeparacao { get; set; }

        public string UsuarioSeparacao { get; set; }
    }
}