using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ProdutoEnderecoRepository : GenericRepository<ProdutoEndereco>
    {
        public ProdutoEnderecoRepository(Entities entities) : base(entities) { }

        public IEnumerable<ProdutoEndereco> Tabela()
        {
            return Entities.ProdutoEndereco;
        }
    }
}