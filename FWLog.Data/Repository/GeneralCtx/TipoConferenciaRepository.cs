using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TipoConferenciaRepository : GenericRepository<TipoConferencia>
    {
        public TipoConferenciaRepository(Entities entities) : base(entities) { }

        public List<TipoConferencia> RetornarTodos()
        {
            return Entities.TipoConferencia.ToList();
        }
    }
}
