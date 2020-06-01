using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class SalvarSeparacaoProdutoRequisicao
    {
        [Required(ErrorMessage = "O IdPedidoVendaVolume deve ser informado.")]
        public long IdPedidoVendaVolume { get; set; }

        [Required(ErrorMessage = "O IdProduto deve ser informado.")]
        public long IdProduto { get; set; }

        public long? IdProdutoSeparacao { get; set; }

        public int? QtdAjuste { get; set; }

        public string CodigoUsuarioAutorizacaoZerarPedido { get; set; }
    }
}