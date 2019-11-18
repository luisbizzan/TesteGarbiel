using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaFiscalStatusRepository : GenericRepository<NotaFiscalStatus>
    {
        public NotaFiscalStatusRepository(Entities entities) : base(entities) { }
    }
}
