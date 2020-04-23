using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoItemRepository : GenericRepository<PedidoItem>
    {
        public PedidoItemRepository(Entities entities) : base(entities)
        {

        }
    }
}
