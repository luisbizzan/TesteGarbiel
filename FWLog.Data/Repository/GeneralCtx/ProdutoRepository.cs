using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ProdutoRepository : GenericRepository<Produto>
    {
        public ProdutoRepository(Entities entities) : base(entities)
        {

        }

        public Produto ConsultarPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.Produto.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }

        public Produto ConsultarPorCodigoBarras(string codigoBarras)
        {
            return Entities.Produto.FirstOrDefault(f => f.CodigoBarras == codigoBarras);
        }

        public IQueryable<Produto> Todos()
        {
            return Entities.Produto;
        }
    }
}
