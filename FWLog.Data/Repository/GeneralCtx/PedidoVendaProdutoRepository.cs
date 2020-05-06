using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaProdutoRepository : GenericRepository<PedidoVendaProduto>
    {
        public PedidoVendaProdutoRepository(Entities entities) : base(entities)
        {
        }

        public List<PedidoVendaProduto> ObterPorIdPedidoVenda(long idPedidoVenda)
        {
            return Entities.PedidoVendaProduto.Where(x => x.IdPedidoVenda == idPedidoVenda).ToList();
        }

        public PedidoVendaProduto ObterPorIdProduto(long idProduto)
        {
            return Entities.PedidoVendaProduto.FirstOrDefault(x => x.IdProduto == idProduto);
        }

        public PedidoVendaProduto ObterPorIdPedidoVendaEIdProduto(long idPedidoVenda, long idProduto)
        {
            return Entities.PedidoVendaProduto.FirstOrDefault(pvp => pvp.IdPedidoVenda == idPedidoVenda && pvp.IdProduto == idProduto);
        }
    }
}