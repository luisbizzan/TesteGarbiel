using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class QuarentenaStatusRepository : GenericRepository<QuarentenaStatus>
    {
        public QuarentenaStatusRepository(Entities entities) : base(entities)
        {

        }

        public IEnumerable<QuarentenaStatus> Todos()
        {
            return Entities.QuarentenaStatus;
        }
    }
}
