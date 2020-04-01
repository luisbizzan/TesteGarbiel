using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaProdutoRepository : GenericRepository<GarantiaProduto>
    {
        public GarantiaProdutoRepository(Entities entities) : base(entities) { }

        public List<GarantiaProduto> ObterPorProduto(long idGarantia, long idProduto)
        {
            return Entities.GarantiaProduto.Where(w => w.IdGarantia == idGarantia && w.IdProduto == idProduto).ToList();
        }

        public GarantiaProduto PegarGarantiaProduto(long idGarantia, long idProduto)
        {
            return Entities.GarantiaProduto.FirstOrDefault(w => w.IdGarantia == idGarantia && w.IdProduto == idProduto);
        }
    }
}
