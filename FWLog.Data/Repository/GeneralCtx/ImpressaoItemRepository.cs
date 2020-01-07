using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ImpressaoItemRepository : GenericRepository<ImpressaoItem>
    {
        public ImpressaoItemRepository(Entities entities) : base(entities)
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

        public ImpressaoItem Obter(int idImpressaoItem)
        {
            return Entities.ImpressaoItem.FirstOrDefault(f => (int)f.IdImpressaoItem == idImpressaoItem);
        }
    }
}
