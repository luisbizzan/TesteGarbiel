using FWLog.Services.Model.Caixa;
using FWLog.Services.Model.Produto;
using System.Collections.Generic;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class PedidoItemViewModel
    {
        public ProdutoViewModel Produto { get; set; }
        public List<CaixaViewModel> Caixa { get; set; }
        public int Agrupador { get; set; }
        public CaixaViewModel CaixaEscolhida { get; set; }
        public long IdGrupoCorredorArmazenagem { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public int Quantidade { get; set; }
    }
}
