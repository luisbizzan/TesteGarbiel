using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class BOPrinterRepository : GenericRepository<Printer>
    {
        public BOPrinterRepository(Entities entities) : base(entities)
        {
        }

        public IQueryable<Printer> All(ReadOnlyCollection<long> IdEmpresas)
        {
            return _dbSet.Include(x => x.Empresa).Include(x => x.PrinterType).Where(x => IdEmpresas.Contains(x.Empresa.IdEmpresa));
        }
    }
}
