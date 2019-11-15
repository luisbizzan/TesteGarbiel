using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaFiscalItemRepository : GenericRepository<NotaFiscalItem>
    {
        public NotaFiscalItemRepository(Entities entities) : base(entities)
        {

        }

        public NotaFiscalItem PegarNotaFiscal(long codigoNotaFiscal, long idProduto)
        {
            return Entities.NotaFiscalItem.FirstOrDefault(f => f.CodigoNotaFiscal == codigoNotaFiscal && f.IdProduto == idProduto);
        }

        public List<NotaFiscalItem> ObterItens(long idNotaFiscal)
        {
            return Entities.NotaFiscalItem.Include("Produto").Where(w => w.IdNotaFiscal == idNotaFiscal).ToList();
        }
    }
}
