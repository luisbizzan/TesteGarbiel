using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteStatusRepository : GenericRepository<LoteStatus>
    {
        public LoteStatusRepository(Entities entities) : base(entities) { }
    }
}
