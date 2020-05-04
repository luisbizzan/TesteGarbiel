using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoRepository : GenericRepository<Pedido>
    {
        public PedidoRepository(Entities entities) : base(entities)
        {

        }

        public Pedido ObterPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.Pedido.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }

        public List<Pedido> ObterPedidosSemNotaFiscal()
        {
            var query = Entities.Pedido.Where(f => f.CodigoIntegracaoNotaFiscal  == null);

            return query.ToList();
        }
    }
}