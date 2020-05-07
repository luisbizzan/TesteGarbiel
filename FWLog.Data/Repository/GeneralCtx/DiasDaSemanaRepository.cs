using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class DiasDaSemanaRepository : GenericRepository<DiasDaSemana>
    {
        public DiasDaSemanaRepository(Entities entities) : base(entities)
        {

        }

        public List<DiasDaSemana> RetornarTodos()
        {
           return Entities.DiasDaSemana.ToList();
        }
    }
}
