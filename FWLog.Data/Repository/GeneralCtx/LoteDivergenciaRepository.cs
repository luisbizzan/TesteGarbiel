using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteDivergenciaRepository : GenericRepository<LoteDivergencia>
    {
        public LoteDivergenciaRepository(Entities entities) : base(entities) { }
    }
}
