using FWLog.Services.Model.ProdutoEstoque;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class LoteProdutoEnderecoViewModel
    {
        public long Id { get; set; }
        public int Quantidade { get; set; }
        public ProdutoEstoqueViewModel EnderecoSeparacao { get; set; }
        public bool IsSeparacaoNoPikcing { get; set; }
    }
}
