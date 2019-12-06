using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteDivergenciaRepository : GenericRepository<LoteDivergencia>
    {
        public LoteDivergenciaRepository(Entities entities) : base(entities) { }

        public List<LoteDivergencia> RetornarPorNotaFiscal(long idNotaFiscal)
        {
            return Entities.LoteDivergencia.AsNoTracking()
                .Include("Lote")
                .Include("NotaFiscal")
                .Include("Produto")
                .Include("LoteDivergenciaStatus")
                .Where(w => w.IdNotaFiscal == idNotaFiscal).ToList();
        }
    }
}
