using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class RomaneioRepository : GenericRepository<Romaneio>
    {
        public RomaneioRepository(Entities entities) : base(entities)
        {

        }
    }
}
