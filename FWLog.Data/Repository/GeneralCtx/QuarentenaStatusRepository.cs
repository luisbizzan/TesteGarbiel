using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class QuarentenaStatusRepository : GenericRepository<QuarentenaStatus>
    {
        public QuarentenaStatusRepository(Entities entities) : base(entities)
        {

        }
    }
}
