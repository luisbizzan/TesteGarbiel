using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TipoMovimentacaoRepository : GenericRepository<TipoMovimentacao>
    {
        public TipoMovimentacaoRepository(Entities entities) : base(entities)
        {

        }

        public List<TipoMovimentacao> RetornarTodos()
        {
            return Entities.TipoMovimentacao.ToList();
        }
    }
}
