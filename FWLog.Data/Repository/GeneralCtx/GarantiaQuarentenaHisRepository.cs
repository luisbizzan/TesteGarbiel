using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaQuarentenaHisRepository : GenericRepository<GarantiaQuarentenaHis>
    {
        public GarantiaQuarentenaHisRepository(Entities entities) : base(entities) { }
    }
}
