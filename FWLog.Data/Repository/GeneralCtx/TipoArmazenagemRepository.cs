using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TipoArmazenagemRepository : GenericRepository<TipoArmazenagem>
    {
        public TipoArmazenagemRepository(Entities entities) : base(entities)
        {

        }

        public List<TipoArmazenagem> RetornarTodos()
        {
            return Entities.TipoArmazenagem.ToList();
        }
    }
}
