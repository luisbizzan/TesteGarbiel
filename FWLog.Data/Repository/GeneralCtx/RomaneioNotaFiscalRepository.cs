using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class RomaneioNotaFiscalRepository : GenericRepository<RomaneioNotaFiscal>
    {
        public RomaneioNotaFiscalRepository(Entities entities) : base(entities)
        {
        }

        public RomaneioNotaFiscal BuscarPorRomaneioENumeroNotaFiscal(int nroRomaneio, int nroNotaFiscal)
        {
            return Entities.RomaneioNotaFiscal.Where(romaneioNF => romaneioNF.Romaneio.NroRomaneio == nroRomaneio && romaneioNF.NroNotaFiscal == nroNotaFiscal).FirstOrDefault();
        }
    }
}