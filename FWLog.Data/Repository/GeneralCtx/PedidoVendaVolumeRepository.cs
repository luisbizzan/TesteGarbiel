using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaVolumeRepository : GenericRepository<PedidoVendaVolume>
    {
        public PedidoVendaVolumeRepository(Entities entities) : base(entities)
        {
        }

        public List<PedidoVendaVolume> ObterPorIdsPedidoVendaVolume(List<long> idsPedidoVendaVolume)
        {
            return Entities.PedidoVendaVolume.Where(x => idsPedidoVendaVolume.Contains(x.IdPedidoVendaVolume)).ToList();
        }

        public List<PedidoVendaVolume> ObterPorIdPedidoVenda(long idPedidoVenda)
        {
            return Entities.PedidoVendaVolume.Where(x => x.IdPedidoVenda == idPedidoVenda).ToList();
        }

        public List<PedidoVendaVolume> ObterVolumesInstaladosPorTranportadoraEmpresa(long idTransportadora, long idEmpresa)
        {
            var query = Entities.PedidoVendaVolume.Where(pedidoVendaVolume => pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora && pedidoVendaVolume.PedidoVenda.IdTransportadora == idTransportadora && pedidoVendaVolume.PedidoVenda.IdEmpresa == idEmpresa);

            return query.ToList();
        }
    }
}