using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.ExpedicaoPedido
{
    public class IniciarExpedicaoRequisicao
    {
        [Required]
        public long IdPedidoVenda { get; set; }

        [Required]
        public long IdPedidoVendaVolume { get; set; }
    }
}