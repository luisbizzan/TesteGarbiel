using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class BOPrinterTypeRepository : GenericRepository<PrinterType>
    {
        public BOPrinterTypeRepository(Entities entities) : base(entities)
        {
        }
    }
}
