using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class FornecedorRepository : GenericRepository<Fornecedor>
    {
        public FornecedorRepository(Entities entities) : base(entities)
        {

        }

        public IEnumerable<Fornecedor> Todos()
        {
            return Entities.Fornecedor;
        }

        public Fornecedor ConsultarPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.Fornecedor.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }
    }
}
