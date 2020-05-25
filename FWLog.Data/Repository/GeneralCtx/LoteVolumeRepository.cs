using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteVolumeRepository : GenericRepository<LoteVolume>
    {
        public LoteVolumeRepository(Entities entities) : base(entities)
        {

        }

        public LoteVolume Obter(long idLote, int nroVolume)
        {
            return Entities.LoteVolume.FirstOrDefault(f => f.IdLote == idLote && f.NroVolume == nroVolume);
        }
    }
}
