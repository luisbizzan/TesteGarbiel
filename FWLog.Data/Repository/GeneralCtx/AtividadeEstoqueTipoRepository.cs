using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class AtividadeEstoqueTipoRepository : GenericRepository<AtividadeEstoqueTipo>
    {
        public AtividadeEstoqueTipoRepository(Entities entities) : base(entities) { }

        public IEnumerable<AtividadeEstoqueTipo> Todos()
        {
            return Entities.AtividadeEstoqueTipo.ToList();
        }

    }
}
