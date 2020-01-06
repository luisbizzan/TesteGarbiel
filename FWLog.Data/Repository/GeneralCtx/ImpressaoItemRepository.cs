using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ImpressoraItemRepository : GenericRepository<ImpressaoItem>
    {
        public ImpressoraItemRepository(Entities entities) : base(entities)
        {

        }

        public List<ImpressaoItem> Todos()
        {
            return Entities.ImpressaoItem.ToList();
        }

        public IQueryable<ImpressaoItem> Tabela()
        {
            return Entities.ImpressaoItem;
        }
    }
}
