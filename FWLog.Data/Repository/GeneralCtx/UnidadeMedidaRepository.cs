using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class UnidadeMedidaRepository : GenericRepository<UnidadeMedida>
    {
        public UnidadeMedidaRepository(Entities entities) : base(entities)
        {

        }
    }
}
