using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoStatusRepository : GenericRepository<PedidoStatus>
    {
        public PedidoStatusRepository(Entities entities) : base(entities)
        {
        }
    }
}
