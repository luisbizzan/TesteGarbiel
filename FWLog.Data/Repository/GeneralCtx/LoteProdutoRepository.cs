using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteProdutoRepository : GenericRepository<LoteProduto>
    {
        public LoteProdutoRepository(Entities entities) : base(entities) { }
    }
}
