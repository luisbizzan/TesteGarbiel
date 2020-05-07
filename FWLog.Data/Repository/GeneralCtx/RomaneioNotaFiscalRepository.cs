using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class RomaneioNotaFiscalRepository : GenericRepository<RomaneioNotaFiscal>
    {
        public RomaneioNotaFiscalRepository(Entities entities) : base(entities)
        {
        }

        public List<RomaneioNotaFiscal> BuscarPorRomaneioENumeroNotaFiscal(int nroRomaneio, int nroNotaFiscal)
        {
            return Entities.RomaneioNotaFiscal.Where(romaneioNF => romaneioNF.Romaneio.NroRomaneio == nroRomaneio && romaneioNF.NroNotaFiscal == nroNotaFiscal).ToList();
        }
    }
}