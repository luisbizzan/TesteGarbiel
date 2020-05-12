using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class RomaneioRepository : GenericRepository<Romaneio>
    {
        public RomaneioRepository(Entities entities) : base(entities)
        {
        }

        public Romaneio BuscarPorNumeroRomaneioEEmpresa(int nroRomaneio, long idEmpresa)
        {
            return Entities.Romaneio.Where(romaneio => romaneio.NroRomaneio == nroRomaneio && romaneio.IdEmpresa == idEmpresa).FirstOrDefault();
        }

        public Romaneio BuscarPorIdRomaneioEEmpresa(long idRomaneio, long idEmpresa)
        {
            return Entities.Romaneio.Where(romaneio => romaneio.IdRomaneio == idRomaneio && romaneio.IdEmpresa == idEmpresa).FirstOrDefault();
        }

        public int BuscaUltimoNroRomaneioPorEmpresa(long idEmpresa)
        {
            return Entities.Romaneio.Where(romaneio => romaneio.IdEmpresa == idEmpresa).Max(x => x.NroRomaneio);
        }
    }
}