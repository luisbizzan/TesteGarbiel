using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class IntegracaoLogRepository : GenericRepository<IntegracaoLog>
    {
        public IntegracaoLogRepository(Entities entities) : base(entities) { }

        public List<IntegracaoLog> Todos()
        {
            return Entities.IntegracaoLog.ToList();
        }

   
    }
}
