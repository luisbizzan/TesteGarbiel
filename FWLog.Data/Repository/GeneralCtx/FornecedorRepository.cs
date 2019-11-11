using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class FornecedorRepository : GenericRepository<Fornecedor>
    {
        public FornecedorRepository(Entities entities) : base(entities)
        {

        }
    }
}
