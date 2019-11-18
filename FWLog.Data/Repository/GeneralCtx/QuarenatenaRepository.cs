using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Data.Entity;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class QuarentenaRepository : GenericRepository<Quarentena>
    {
        public QuarentenaRepository(Entities entities) : base(entities)
        {

        }

        public override IQueryable<Quarentena> All()
        {
            return _dbSet.Include(x => x.Lote).Include(x => x.QuarentenaStatus);
        }
    }
}
