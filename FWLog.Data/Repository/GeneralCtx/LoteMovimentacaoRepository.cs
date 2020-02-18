using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteMovimentacaoRepository : GenericRepository<LoteMovimentacao>
    {
        public LoteMovimentacaoRepository(Entities entities) : base(entities) { }

    }
}
