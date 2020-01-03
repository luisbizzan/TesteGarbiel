using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PerfilImpressoraItemRepository : GenericRepository<PerfilImpressoraItem>
    {
        public PerfilImpressoraItemRepository(Entities entities) : base(entities)
        {

        }
    }
}
