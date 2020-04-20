using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaProdutoStatusRepository : GenericRepository<PedidoVendaProdutoStatus>
    {
        public PedidoVendaProdutoStatusRepository(Entities entities) : base(entities)
        {

        }
    }
}
