using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteProdutoRepository : GenericRepository<LoteProduto>
    {
        public LoteProdutoRepository(Entities entities) : base(entities) { }

        public LoteProduto ConsultarPorLote(long idLote)
        {
            return Entities.LoteProduto.Where(x => x.IdLote == idLote).FirstOrDefault();
        }

        public LoteProduto ConsultarPorLoteProduto(long idLote, long idProduto)
        {
            return Entities.LoteProduto.Where(loteProduto => loteProduto.IdLote == idLote && loteProduto.IdProduto == idProduto).FirstOrDefault();
        }

        public LoteProduto PesquisarProdutoNoLote(long idEmpresa, long idLote, long idProduto)
        {
            return Entities.LoteProduto.Where(w => w.IdEmpresa == idEmpresa && w.IdLote == idLote && w.IdProduto == idProduto).FirstOrDefault();
        }

        public LoteProduto PesquisarProdutoMaisAntigoPorEmpresaESaldo(long idProduto, long idEmpresa)
        {
            return Entities.LoteProduto.Where(loteProduto => loteProduto.IdProduto == idProduto &&
                                                                loteProduto.IdEmpresa == idEmpresa &&
                                                                loteProduto.Saldo > 0)
                .OrderBy(loteProduto => loteProduto.Lote.DataRecebimento)
                .FirstOrDefault();
        }
    }
}
