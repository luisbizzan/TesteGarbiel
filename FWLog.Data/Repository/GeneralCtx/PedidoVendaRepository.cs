using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
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

        public List<PedidoVendaItem> BuscarDadosPedidoVendaParaTabela(DataTableFilter<PedidoVendaFiltro> filtro, out int registrosFiltrados, out int totalRegistros)
        {
            var totalQuery = Entities.PedidoVenda.AsNoTracking().Where(pvv => pvv.IdEmpresa == filtro.CustomFilter.IdEmpresa);

            totalRegistros = totalQuery.Count();

            var baseQuery = totalQuery;

            if (filtro.CustomFilter.NumeroPedido.HasValue)
            {
                var numeroPedido = filtro.CustomFilter.NumeroPedido.Value;

                baseQuery = baseQuery.Where(pedidoVenda => pedidoVenda.Pedido.NroPedido == numeroPedido);
            }

            if (filtro.CustomFilter.NumeroPedidoVenda.HasValue)
            {
                var numeroPedidoVenda = filtro.CustomFilter.NumeroPedidoVenda.Value;

                baseQuery = baseQuery.Where(pedidoVenda => pedidoVenda.NroPedidoVenda == numeroPedidoVenda);
            }

            if (!filtro.CustomFilter.NomeTransportadora.NullOrEmpty())
            {
                baseQuery = baseQuery.Where(pedidoVenda => pedidoVenda.Transportadora.NomeFantasia.Contains(filtro.CustomFilter.NomeTransportadora));
            }

            var query = baseQuery.Select(pedidoVenda => new PedidoVendaItem
            {
                IdPedidoVenda = pedidoVenda.IdPedidoVenda,
                NumeroPedido = pedidoVenda.Pedido.NroPedido,
                NumeroPedidoVenda = pedidoVenda.NroPedidoVenda,
                ClienteNome = pedidoVenda.Cliente.NomeFantasia,
                TransportadoraNome = pedidoVenda.Transportadora.NomeFantasia
            });

            registrosFiltrados = query.Count();

            var response = query.OrderBy(filtro.OrderByColumn, filtro.OrderByDirection)
                                .Skip(filtro.Start)
                                .Take(filtro.Length);

            return response.ToList();
        }

        public List<PedidoVenda> BuscarPedidosExpedidosPorEmpresa(long idEmpresa)
        {
            return Entities.PedidoVenda.AsNoTracking()
                .Where(pv => pv.IdPedidoVendaStatus == PedidoVendaStatusEnum.RomaneioImpresso &&
                             pv.IdEmpresa == idEmpresa &&
                             pv.Pedido.NumeroNotaFiscal.HasValue && 
                             pv.Pedido.SerieNotaFiscal != null).ToList();
        }

        public IQueryable<PedidoVendaVolume> BuscarPedidosVolumePorEmpresa(long idEmpresa)
        {
            return Entities.PedidoVendaVolume.Where(w => w.PedidoVenda.IdEmpresa == idEmpresa);
        }
    }
}