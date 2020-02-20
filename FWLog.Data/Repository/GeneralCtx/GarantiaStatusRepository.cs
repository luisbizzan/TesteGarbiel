using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaStatusRepository : GenericRepository<GarantiaStatus>
    {
        public GarantiaStatusRepository(Entities entities) : base(entities) { }

        public IEnumerable<GarantiaStatus> Todos()
        {
            return Entities.GarantiaStatus;
        }
    }
}
