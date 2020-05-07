using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class RomaneioNotaFiscalRepository : GenericRepository<RomaneioNotaFiscal>
    {
        public RomaneioNotaFiscalRepository(Entities entities) : base(entities)
        {

        }
    }
}
