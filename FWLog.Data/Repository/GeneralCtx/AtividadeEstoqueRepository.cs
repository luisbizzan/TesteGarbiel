using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class AtividadeEstoqueRepository : GenericRepository<AtividadeEstoque>
    {
        public AtividadeEstoqueRepository(Entities entities) : base(entities) { }

    }
}
