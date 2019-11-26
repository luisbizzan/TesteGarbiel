using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;

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
    }
}
