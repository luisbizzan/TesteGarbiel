using ExtensionMethods.List;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class BOPrinterTypeRepository : GenericRepository<PrinterType>
    {
        public BOPrinterTypeRepository(Entities entities) : base(entities)
        {
        }

        public IEnumerable<PrinterType> Todos()
        {
            return Entities.PrinterType;
        }
    }
}
