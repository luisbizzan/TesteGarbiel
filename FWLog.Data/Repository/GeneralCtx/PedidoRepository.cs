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

        public Pedido ObterPorNumero(long idEmpresa, string numeroPedido)
        {
            return Entities.Pedido.FirstOrDefault(f => f.IdEmpresa == idEmpresa && f.NumeroPedido == numeroPedido);
        }

        public List<Pedido> PesquisarProcessandoSeparacao(long idEmpresa)
        {
            return Entities.Pedido.Where(pv => pv.IdEmpresa == idEmpresa && pv.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao).ToList();
        }

        public List<Pedido> PesquisarPendenteSeparacao(long idEmpresa)
        {
            return Entities.Pedido.Where(pv => pv.IdEmpresa == idEmpresa && pv.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteSeparacao).ToList();
        }

        public int PesquisarTotalPendenteSeparacao(long idEmpresa)
        {
            return Entities.Pedido.Count(pv => pv.IdEmpresa == idEmpresa && pv.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteSeparacao);
        }

        public List<Pedido> ObterPedidosSemNotaFiscal()
        {
            var query = Entities.Pedido.Where(f => f.CodigoIntegracaoNotaFiscal == null);

            return query.ToList();
        }

        public Pedido PesquisaPorChaveAcesso(string chaveAcesso)
        {
            return Entities.Pedido.FirstOrDefault(f => f.ChaveAcessoNotaFiscal == chaveAcesso);
        }
    }
}