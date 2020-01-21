using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ProdutoEmpresaRepository : GenericRepository<ProdutoEmpresa>
    {
        public ProdutoEmpresaRepository(Entities entities) : base(entities)
        {

        }

        public ProdutoEmpresa ConsultarPorProduto(long idProduto)
        {
            return Entities.ProdutoEmpresa.Where(x => x.IdProduto == idProduto).FirstOrDefault();
        }

        public ProdutoEmpresa ConsultarPorProdutoEmpresa(long idProduto, long idEmpresa)
        {
            return Entities.ProdutoEmpresa.Where(x => x.IdProduto == idProduto && x.IdEmpresa == idEmpresa).FirstOrDefault();
        }
    }
}
