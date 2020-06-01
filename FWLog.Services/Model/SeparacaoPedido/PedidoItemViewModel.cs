using FWLog.Services.Model.Caixa;
using FWLog.Services.Model.GrupoCorredorArmazenagem;
using FWLog.Services.Model.Produto;
using FWLog.Services.Model.ProdutoEstoque;
using System.Collections.Generic;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class PedidoItemViewModel
    {
        public ProdutoViewModel Produto { get; set; } //Produtos.
        public List<CaixaViewModel> Caixa { get; set; } //Lista de caixas que poderão ser utilizadas para separação do produto.
        public int Agrupador { get; set; } //Usado na lógica da seleção da melhor caixa.
        public CaixaViewModel CaixaEscolhida { get; set; } //Caixa que foi escolhida para o produto.
        public GrupoCorredorArmazenagemViewModel GrupoCorredorArmazenagem { get; set; }
        public ProdutoEstoqueViewModel EnderecoSeparacao { get; set; } //Endereço de separação.
        public int Quantidade { get; set; } //Quantidades do produto.
        public bool IsSeparacaoNoPikcing { get; set; }
        public long? IdLote { get; set; }
    }
}
