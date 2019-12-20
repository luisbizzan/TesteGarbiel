using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class FreteTipoRepository : GenericRepository<FreteTipo>
    {
        public FreteTipoRepository(Entities entities) : base(entities)
        {

        }

        public List<FreteTipo> RetornarTodos()
        {
            return Entities.FreteTipo.ToList();
        }

        public FreteTipo ConsultarPorSigla(string sigla)
        {
            return Entities.FreteTipo.FirstOrDefault(f => f.Sigla == sigla);
        }
    }
}
