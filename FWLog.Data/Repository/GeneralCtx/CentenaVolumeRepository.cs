using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class CentenaVolumeRepository : GenericRepository<CentenaVolume>
    {
        public CentenaVolumeRepository(Entities entities) : base(entities) { }

        public CentenaVolume ConsultarPorEmpresa(long idEmpresa)
        {
            return Entities.CentenaVolume.FirstOrDefault(x => x.IdEmpresa == idEmpresa);
        }
    }
}