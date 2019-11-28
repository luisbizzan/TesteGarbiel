using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EnderecoArmazenagemRepository : GenericRepository<EnderecoArmazenagem>
    {
        public EnderecoArmazenagemRepository(Entities entities) : base(entities) { }
    }
}
