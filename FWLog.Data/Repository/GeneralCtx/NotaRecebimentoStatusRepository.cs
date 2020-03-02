using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaRecebimentoStatusRepository : GenericRepository<NotaRecebimentoStatus>
    {
        public NotaRecebimentoStatusRepository(Entities entities) : base(entities) { }
    }
}
