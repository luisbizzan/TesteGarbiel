using FWLog.Data.Models;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class SalvarSeparacaoProdutoResposta
    {
        public long IdPedidoVenda { get; set; }
        public long IdProduto { get; set; }
        public string Referencia { get; set; }
        public int QtdeSeparar { get; set; }
        public int QtdeSeparada { get; set; }
        public decimal? Multiplo { get; set; }
        public FWLog.Data.Models.Produto ProdutoSeparado { get; set; }
        public PedidoVendaVolume Volume { get; set; }
    }
}
