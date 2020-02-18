using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteMovimentacaoTipoRepository : GenericRepository<LoteMovimentacaoTipo>
    {
        public LoteMovimentacaoTipoRepository(Entities entities) : base(entities) { }

    }
}
