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
    }
}