using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteStatusRepository : GenericRepository<LoteStatus>
    {
        public LoteStatusRepository(Entities entities) : base(entities) { }

        public IEnumerable<LoteStatus> Todos()
        {
            return Entities.LoteStatus;
        }
    }
}
