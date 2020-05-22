using System.Collections.Generic;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class DetalhePedidoVendaResposta
    {
        public long IdPedidoVenda { get; set; }

        public int NroPedidoVenda { get; set; }

        public string Status { get; set; }

        public int NroVolumes { get; set; }

        public List<DetalhePedidoVendaItemResposta> ListaProdutos { get; set; }
    }

    public class DetalhePedidoVendaItemResposta
    {
        public string ReferenciaProduto { get; set; }

        public int QuantidadeSeparar { get; set; }

        public int QuantidadeSeparada { get; set; }
    }
}