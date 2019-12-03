using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class QuarentenaRepository : GenericRepository<Quarentena>
    {
        public QuarentenaRepository(Entities entities) : base(entities)
        {
        }

        public IQueryable<Quarentena> All()
        {
            return _dbSet.Include(x => x.Lote).Include(x => x.QuarentenaStatus);
        }

        public bool Any(Expression<Func<Quarentena, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }
    }
}
