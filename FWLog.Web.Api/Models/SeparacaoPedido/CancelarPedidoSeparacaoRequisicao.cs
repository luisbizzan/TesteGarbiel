using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class CancelarPedidoSeparacaoRequisicao
    {
        [Required]
        public long IdPedidoVenda { get; set; }

        [Required]
        public string UsuarioPermissao { get; set; }
    }
}