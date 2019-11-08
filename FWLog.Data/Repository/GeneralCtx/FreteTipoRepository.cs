using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class FreteTipoRepository : GenericRepository<FreteTipo>
    {
        public FreteTipoRepository(Entities entities) : base(entities)
        {

        }

        public void Teste()
        {
            Entities.FreteTipo.ToList();
        }
    }
}
