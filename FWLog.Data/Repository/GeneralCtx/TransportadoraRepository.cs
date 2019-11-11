using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TransportadoraRepository : GenericRepository<Transportadora>
    {
        public TransportadoraRepository(Entities entities) : base(entities)
        {

        }
    }
}
