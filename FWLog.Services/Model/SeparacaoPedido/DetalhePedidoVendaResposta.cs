using System;
using System.Collections.Generic;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class DetalhePedidoVendaResposta
    {
        public long IdPedidoVenda { get; set; }

        public int NroPedidoVenda { get; set; }

        public DateTime DataCriacao { get; set; }

        public string Status { get; set; }

        public decimal PesoTotal { get; set; }

        public List<DetalhePedidoVendaVolumeResposta> ListaVolumes { get; set; }
    }

    public class DetalhePedidoVendaVolumeResposta
    {
        public long IdPedidoVendaVolume { get; set; }

        public int Numero { get; set; }

        public string Status { get; set; }

        public List<DetalhePedidoVendaVolumeProdutoResposta> ListaProdutos { get; set; }
    }

    public class DetalhePedidoVendaVolumeProdutoResposta
    {
        public long IdPedidoVendaProduto { get; set; }

        public long IdProduto { get; set; }

        public string ReferenciaProduto { get; set; }

        public string CodigoEndereco { get; set; }

        public string DescricaoPontoArmazenagem { get; set; }

        public int QuantidadeSeparar { get; set; }

        public int QuantidadeSeparada { get; set; }

        public int? Corredor { get; set; }

        public string UsuarioSeparacao { get; set; }

        public DateTime? DataHoraSeparacao { get; set; }

        public decimal Peso { get; set; }
    }
}