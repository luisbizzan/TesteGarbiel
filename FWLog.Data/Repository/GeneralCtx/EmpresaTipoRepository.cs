using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EmpresaTipoRepository : GenericRepository<EmpresaTipo>
    {
        public EmpresaTipoRepository(Entities entities) : base(entities) { }

        public List<EmpresaTipo> RetornarTodos()
        {
            return Entities.EmpresaTipo.ToList();
        }
    }
}
