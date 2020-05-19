using FWLog.Services.Model.SeparacaoPedido;
using System.Collections.Generic;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class ConsultaPedidoVendaEmSeparacaoResposta
    {
        public List<PedidoVendaVolumeEmSeparacaoViewModel> PedidosVendaVolumeEmSeparacao { get; set; }
    }
}