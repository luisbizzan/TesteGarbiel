using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class CancelarPedidoSeparacaoRequisicao
    {
        [Required]
        public long IdPedidoVenda { get; set; }

        [Required]
        public string IdUsuarioPermissao { get; set; }
    }
}