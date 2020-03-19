using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaRecebimentoStatusRepository : GenericRepository<NotaRecebimentoStatus>
    {
        public NotaRecebimentoStatusRepository(Entities entities) : base(entities) { }

        public IEnumerable<NotaRecebimentoStatus> Todos()
        {
            return Entities.NotaRecebimentoStatus;
        }
    }
}
