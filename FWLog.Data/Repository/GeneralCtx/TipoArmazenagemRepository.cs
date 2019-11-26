using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TipoArmazenagemRepository : GenericRepository<TipoArmazenagem>
    {
        public TipoArmazenagemRepository(Entities entities) : base(entities)
        {

        }
    }
}
