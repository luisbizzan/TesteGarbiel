using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoItemRepository : GenericRepository<PedidoItem>
    {
        public PedidoItemRepository(Entities entities) : base(entities) { }

        public List<PedidoItem> BuscarPorIdPedido(long idPedido)
        {
            return Entities.PedidoItem.Where(x => x.IdPedido == idPedido).ToList();
        }

        public List<PedidoItem> BuscarParaSeparacao(long idPedido)
        {
            return Entities.PedidoItem.Where(w => w.IdPedido == idPedido && w.QtdPedido > 0).ToList();
        }
    }
}
