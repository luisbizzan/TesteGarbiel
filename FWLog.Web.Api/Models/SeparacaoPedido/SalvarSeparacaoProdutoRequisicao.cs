using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class SalvarSeparacaoProdutoRequisicao
    {
        [Required(ErrorMessage = "O id do pedido venda deve ser informado.")]
        public long IdPedidoVenda { get; set; }
        [Required(ErrorMessage = "O id do produto deve ser informado.")]
        public long IdProduto { get; set; }
    }
}