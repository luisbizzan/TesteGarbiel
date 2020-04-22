using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaRepository : GenericRepository<PedidoVenda>
    {
        public PedidoVendaRepository(Entities entities) : base(entities)
        {

        }

        public PedidoVenda ObterPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.PedidoVenda.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }
    }
}
