using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PerfilImpressoraRepository : GenericRepository<PerfilImpressora>
    {
        public PerfilImpressoraRepository(Entities entities) : base(entities)
        {
        }

        public IQueryable<PerfilImpressora> RetornarAtivas()
        {
            return Entities.PerfilImpressora.Where(w => w.Ativo);
        }
    }
}
