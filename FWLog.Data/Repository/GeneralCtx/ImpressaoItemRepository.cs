using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ImpressaoItemRepository : GenericRepository<ImpressaoItem>
    {
        public ImpressaoItemRepository(Entities entities) : base(entities)
        {

        }
    }
}
