using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaRepository : GenericRepository<PedidoVenda>
    {
        public PedidoVendaRepository(Entities entities) : base(entities)
        {

        }

        public List<long> PesquisarIdsEmSeparacao(string idUsuario, long idEmpresa)
        {
            /* return Entities.PedidoVenda.Where(w => w.IdUsuarioSeparacao == idUsuario && w.IdEmpresa == idEmpresa && w.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao).Select(x => x.IdPedidoVenda).ToList();*/

            throw new NotImplementedException();
        }

        public PedidoVenda ObterPorNroPedidoEEmpresa(int nroPedido, long idEmpresa)
        {
            return Entities.PedidoVenda.FirstOrDefault(f => f.NroPedidoVenda == nroPedido && f.IdEmpresa == idEmpresa);
        }

        public PedidoVenda ObterPorIdPedidoVendaEIdEmpresa(long idPedidoVenda, long idEmpresa)
        {
            return Entities.PedidoVenda.Include("Pedido").FirstOrDefault(pv => pv.IdPedidoVenda == idPedidoVenda && pv.IdEmpresa == idEmpresa);
        }

        public PedidoVenda ObterPorIdPedido(long idPedido)
        {
            return Entities.PedidoVenda.FirstOrDefault(x => x.IdPedido == idPedido);
        }

        public bool ExistemPedidosParaDespachoNaTransportadora(long idTransportadora, long idEmpresa)
        {
            return Entities.PedidoVenda.Any(x => x.IdTransportadora == idTransportadora &&
                                                    x.IdEmpresa == idEmpresa &&
                                                    (x.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovidoDOCA
                                                    || x.IdPedidoVendaStatus == PedidoVendaStatusEnum.DespachandoNF));
        }

        public List<PedidoVenda> ObterPorIdTransportadoraRomaneio(long idTransportadora, long idEmpresa)
        {
            return Entities.PedidoVenda.Where(x => x.IdTransportadora == idTransportadora && x.IdEmpresa == idEmpresa && x.IdPedidoVendaStatus == PedidoVendaStatusEnum.NFDespachada).ToList();
        }
    }
}