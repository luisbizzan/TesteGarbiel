using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class BOPrinterRepository : GenericRepository<Printer>
    {
        public BOPrinterRepository(Entities entities) : base(entities)
        {
        }

        public override IEnumerable<Printer> GetAll()
        {
            return All().ToList();
        }

        public override IQueryable<Printer> All()
        {
            return _dbSet.Include(x => x.Company).Include(x => x.PrinterType);
        }
    }
}
