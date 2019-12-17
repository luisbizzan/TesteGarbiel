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

        public void DeletePorId(long idLote)
        {
            var divergencias = Entities.LoteDivergencia.Where(w => w.IdLote == idLote).ToList();
            if (divergencias != null)
            {
                Entities.LoteDivergencia.RemoveRange(divergencias);
            }
        }
    }
}
