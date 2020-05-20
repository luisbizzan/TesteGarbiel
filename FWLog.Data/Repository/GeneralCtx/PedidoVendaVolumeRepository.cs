using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Data;
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

        public List<PedidoVendaVolume> ObterVolumesInstaladosPorTransportadoraEmpresa(long idTransportadora, long idEmpresa)
        {
            var query = Entities.PedidoVendaVolume.Where(pedidoVendaVolume => pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora && pedidoVendaVolume.PedidoVenda.IdTransportadora == idTransportadora && pedidoVendaVolume.PedidoVenda.IdEmpresa == idEmpresa && pedidoVendaVolume.IdEnderecoArmazTransportadora.HasValue);

            return query.ToList();
        }

        public PedidoVendaVolume ObterPedidoVendaVolumePorIdPorEmpresa(long idPedidoVolume, long idEmpresa)
        {
            return Entities.PedidoVendaVolume.FirstOrDefault(pvv => pvv.IdPedidoVendaVolume == idPedidoVolume && pvv.PedidoVenda.IdEmpresa == idEmpresa);
        }

        public List<PedidoVendaVolume> PesquisarIdsEmSeparacao(string idUsuario, long idEmpresa)
        {
            return Entities.PedidoVendaVolume.Where(w => w.PedidoVenda.IdEmpresa == idEmpresa &&
                                                            w.IdUsuarioSeparacaoAndamento == idUsuario &&
                                                            w.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao).Select(x => x).ToList();
        }

        private IQueryable<RelatorioVolumesInstaladosTransportadoraItem> BuscarDadosVolumePorTransportadoraQuery(RelatorioVolumesInstaladosTransportadoraFiltro filtro)
        {
            var baseQuery = Entities.PedidoVendaVolume.AsNoTracking().Where(lpe => lpe.PedidoVenda.IdEmpresa == filtro.IdEmpresa);

            if (filtro.IdTransportadora.HasValue)
            {
                var idTransportadora = filtro.IdTransportadora.Value;

                baseQuery = baseQuery.Where(pedidoVendaVolume => pedidoVendaVolume.PedidoVenda.IdTransportadora == idTransportadora);
            }

            if (filtro.IdPedidoVenda.HasValue)
            {
                var idPedidoVenda = filtro.IdPedidoVenda.Value;

                baseQuery = baseQuery.Where(pedidoVendaVolume => pedidoVendaVolume.IdPedidoVenda == idPedidoVenda);
            }

            var query = baseQuery.Select(pedidoVendaVolume => new RelatorioVolumesInstaladosTransportadoraItem
            {
                Transportadora = pedidoVendaVolume.PedidoVenda.Transportadora.NomeFantasia,
                CodigoEndereco = pedidoVendaVolume.EnderecoTransportadora.Codigo,
                NumeroPedido = pedidoVendaVolume.PedidoVenda.NroPedidoVenda,
                NumeroVolume = pedidoVendaVolume.NroVolume
            });

            return query;
        }

        public List<RelatorioVolumesInstaladosTransportadoraItem> BuscarDadosVolumePorTransportadora(DataTableFilter<RelatorioVolumesInstaladosTransportadoraFiltro> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PedidoVendaVolume.AsNoTracking().Where(pvv => pvv.PedidoVenda.IdEmpresa == filtro.CustomFilter.IdEmpresa).Count();

            var query = BuscarDadosVolumePorTransportadoraQuery(filtro.CustomFilter);

            totalRecordsFiltered = query.Count();

            var orderByColumn = filtro.OrderByColumn;

            if (orderByColumn == "0")
            {
                orderByColumn = "Transportadora";
            }

            var response = query.OrderBy(orderByColumn, filtro.OrderByDirection)
                                .Skip(filtro.Start)
                                .Take(filtro.Length);

            return response.ToList();
        }
    }
}