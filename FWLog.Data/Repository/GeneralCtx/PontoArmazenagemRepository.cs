using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PontoArmazenagemRepository : GenericRepository<PontoArmazenagem>
    {
        public PontoArmazenagemRepository(Entities entities) : base(entities) { }
    }
}
