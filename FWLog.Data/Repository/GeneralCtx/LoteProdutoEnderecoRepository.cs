using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteProdutoEnderecoRepository : GenericRepository<LoteProdutoEndereco>
    {
        public LoteProdutoEnderecoRepository(Entities entities) : base(entities) { }

        public List<LoteProdutoEndereco> PesquisarPorLoteProduto(long idEmpresa, long idLote, long idProduto)
        {
            return Entities.LoteProdutoEndereco.Where(w => w.IdEmpresa == idEmpresa && w.IdLote == idLote && w.IdProduto == idProduto).ToList();
        }
    }
}
