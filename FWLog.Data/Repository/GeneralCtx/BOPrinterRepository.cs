using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Data.Entity;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class BOPrinterRepository : GenericRepository<Printer>
    {
        public BOPrinterRepository(Entities entities) : base(entities)
        {
        }

        public IQueryable<Printer> All()
        {
            return _dbSet.Include(x => x.Empresa).Include(x => x.PrinterType);
        }
    }
}
