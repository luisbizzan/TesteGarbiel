using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaRepository : GenericRepository<PedidoVenda>
    {
        public PedidoVendaRepository(Entities entities) : base(entities)
        {

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

        public List<TransportadoraVolumeForaDoca> BuscarVolumesForaDoca(long idTransportadora, long idEmpresa)
        {
            var query = Entities.PedidoVendaVolume.Where(pedidoVendaVolume => pedidoVendaVolume.PedidoVenda.IdTransportadora == idTransportadora &&
                                                                                pedidoVendaVolume.PedidoVenda.IdEmpresa == idEmpresa &&
                                                                                pedidoVendaVolume.PedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovendoDOCA &&
                                                                                (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovidoDOCA)).
                                                                                Select(pedidoVendaVolume => new TransportadoraVolumeForaDoca
                                                                                {
                                                                                    IdPedidoVendaVolume = pedidoVendaVolume.IdPedidoVendaVolume,
                                                                                    NumeroPedido = pedidoVendaVolume.PedidoVenda.NroPedidoVenda,
                                                                                    NumeroVolume = pedidoVendaVolume.NroVolume,
                                                                                    EnderecoCodigo = pedidoVendaVolume.EnderecoTransportadora.Codigo
                                                                                });

            return query.ToList();
        }

        public List<PedidoVenda> ObterPorIdTransportadoraRomaneio(long idTransportadora, long idEmpresa)
        {
            return Entities.PedidoVenda.Where(x => x.IdTransportadora == idTransportadora && x.IdEmpresa == idEmpresa && x.IdPedidoVendaStatus == PedidoVendaStatusEnum.NFDespachada).ToList();
        }

        public bool TemPedidoVendaMovidoDoca(long idTransportadora, long idEmpresa)
        {
            return Entities.PedidoVenda.Any(x => x.IdTransportadora == idTransportadora &&
                                                    x.IdEmpresa == idEmpresa &&
                                                    (x.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovendoDOCA ||
                                                    x.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovidoDOCA) &&
                                                    x.PedidoVendaVolumes.Any(v => v.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovidoDOCA));
        }
    }
}