using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaProdutoRepository : GenericRepository<PedidoVendaProduto>
    {
        public PedidoVendaProdutoRepository(Entities entities) : base(entities)
        {

        }
    }
}
