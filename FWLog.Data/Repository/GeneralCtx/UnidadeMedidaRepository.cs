using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class UnidadeMedidaRepository : GenericRepository<UnidadeMedida>
    {
        public UnidadeMedidaRepository(Entities entities) : base(entities)
        {

        }

        public List<UnidadeMedida> RetornarTodos()
        {
            return Entities.UnidadeMedida.ToList();
        }

        public UnidadeMedida ConsultaPorSigla(string sigla)
        {
            return Entities.UnidadeMedida.FirstOrDefault(f => f.Sigla == sigla);
        }
    }
}
