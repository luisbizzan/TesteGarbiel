using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EmpresaConfigRepository : GenericRepository<EmpresaConfig>
    {
        public EmpresaConfigRepository(Entities entities) : base(entities)
        {

        }
    }
}
