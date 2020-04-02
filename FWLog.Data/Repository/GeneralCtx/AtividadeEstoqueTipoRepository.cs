using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class AtividadeEstoqueTipoRepository : GenericRepository<AtividadeEstoqueTipo>
    {
        public AtividadeEstoqueTipoRepository(Entities entities) : base(entities) { }

    }
}
