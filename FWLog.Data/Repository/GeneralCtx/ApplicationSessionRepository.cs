using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ApplicationSessionRepository : GenericRepository<ApplicationSession>
    {
        public ApplicationSessionRepository(Entities entities) : base(entities)
        {

        }
    }
}
