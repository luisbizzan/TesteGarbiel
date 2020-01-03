using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PerfilImpressoraRepository : GenericRepository<PerfilImpressora>
    {
        public PerfilImpressoraRepository(Entities entities) : base(entities)
        {

        }
    }
}
