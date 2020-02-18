using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TipoEtiquetagemRepository : GenericRepository<TipoEtiquetagem>
    {
        public TipoEtiquetagemRepository(Entities entities) : base(entities) { }

        public List<TipoEtiquetagem> Todos()
        {
            return Entities.TipoEtiquetagem.ToList();
        }

   
    }
}
