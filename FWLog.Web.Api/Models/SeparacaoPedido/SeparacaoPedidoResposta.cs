using System.Collections.Generic;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class SeparacaoPedidoResposta
    {
        public SeparacaoPedidoResposta()
        {
            PedidosProcessoDeSeparacao = new List<long>();
        }

        public List<long> PedidosProcessoDeSeparacao { get; set; }
    }
}