using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class CancelarPedidoSeparacaoRequisicao
    {
        [Required]
        public long IdPedidoVendaVolume { get; set; }

        [Required]
        public string UsuarioPermissao { get; set; }
    }
}