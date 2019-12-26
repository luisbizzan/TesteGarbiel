using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ProdutoEmpresaRepository : GenericRepository<ProdutoEmpresa>
    {
        public ProdutoEmpresaRepository(Entities entities) : base(entities)
        {

        }

        public ProdutoEmpresa ObterPorProdutoEmpresa(long idProduto, long idEmpresa)
        {
            return Entities.ProdutoEmpresa.FirstOrDefault(f => f.IdProduto == idProduto && f.IdEmpresa == idEmpresa);
        }
    }
}
