using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class IniciarSeparacaoVolumeRequisicao
    {
        [Required]
        public long IdPedidoVenda { get; set; }

        [Required]
        public long IdPedidoVendaVolume { get; set; }
    }
}