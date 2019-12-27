using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TipoEtiquetagemRepository : GenericRepository<TipoEtiquetagem>
    {
        public TipoEtiquetagemRepository(Entities entities) : base(entities) { }
    }
}
