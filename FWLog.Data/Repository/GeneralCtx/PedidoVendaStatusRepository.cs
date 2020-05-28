using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaStatusRepository : GenericRepository<PedidoVendaStatus>
    {
        public PedidoVendaStatusRepository(Entities entities) : base(entities)
        {

        }

        public List<PedidoVendaStatus> Todos()
        {
            return Entities.PedidoVendaStatus.ToList();
        }
    }
}
