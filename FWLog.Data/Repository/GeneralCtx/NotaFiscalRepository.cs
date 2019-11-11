using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaFiscalRepository : GenericRepository<NotaFiscal>
    {
        public NotaFiscalRepository(Entities entities) : base(entities) { }        
    }
}
