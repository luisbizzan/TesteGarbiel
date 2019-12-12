using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteConferenciaRepository : GenericRepository<LoteConferencia>
    {
        public LoteConferenciaRepository(Entities entities) : base(entities) { }

        public List<LoteConferencia> Obter(long idLote)
        {
            return Entities.LoteConferencia.Where(w => w.IdLote == idLote).ToList();
        }

        public LoteConferencia ObterPorId(long idLote)
        {
            return Entities.LoteConferencia.Where(w => w.IdLote == idLote).FirstOrDefault();
        }
    }
}
