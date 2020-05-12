using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaStatusRepository : GenericRepository<PedidoVendaStatus>
    {
        public PedidoVendaStatusRepository(Entities entities) : base(entities)
        {

        }
    }
}
