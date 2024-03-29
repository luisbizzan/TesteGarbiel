﻿using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System;
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
            var query = Entities.PedidoVendaVolume.Where(pedidoVendaVolume => pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora &&
                                                                                pedidoVendaVolume.PedidoVenda.IdTransportadora == idTransportadora &&
                                                                                pedidoVendaVolume.PedidoVenda.IdEmpresa == idEmpresa &&
                                                                                pedidoVendaVolume.IdEnderecoArmazTransportadora.HasValue);

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
                                                            w.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao).OrderBy(o => o.DataHoraInicioSeparacao).Select(x => x).ToList();
        }

        private IQueryable<RelatorioVolumesInstaladosTransportadoraItem> BuscarDadosVolumePorTransportadoraQuery(RelatorioVolumesInstaladosTransportadoraFiltro filtro)
        {
            var baseQuery = Entities.PedidoVendaVolume.AsNoTracking().Where(lpe => lpe.PedidoVenda.IdEmpresa == filtro.IdEmpresa);

            if (filtro.IdTransportadora.HasValue)
            {
                var idTransportadora = filtro.IdTransportadora.Value;

                baseQuery = baseQuery.Where(pedidoVendaVolume => pedidoVendaVolume.PedidoVenda.IdTransportadora == idTransportadora);
            }

            if (!filtro.EnderecoCodigo.NullOrEmpty())
            {
                baseQuery = baseQuery.Where(pedidoVendaVolume => pedidoVendaVolume.EnderecoTransportadora.Codigo.Equals(filtro.EnderecoCodigo));
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

        public List<RelatorioVolumesInstaladosTransportadoraItem> BuscarDadosVolumePorTransportadora(RelatorioVolumesInstaladosTransportadoraFiltro model)
        {
            var query = BuscarDadosVolumePorTransportadoraQuery(model);

            return query.ToList();
        }

        public List<PedidoVendaVolumeGrupoArmazenagemLinhaTabela> BuscarDadosVolumeGrupoArmazenagem(DateTime dataInicial, DateTime dataFinal, long idEmpresa)
        {
            dataFinal = dataFinal.Date.AddDays(1).Subtract(new TimeSpan(0, 0, 1));

            var query = Entities.PedidoVendaVolume.Where(pedidoVendaVolume => pedidoVendaVolume.PedidoVenda.IdEmpresa == idEmpresa && pedidoVendaVolume.PedidoVenda.Pedido.DataCriacao >= dataInicial && pedidoVendaVolume.PedidoVenda.Pedido.DataCriacao <= dataFinal);

            var selectQuery = query.Select(q => new PedidoVendaVolumeGrupoArmazenagemLinhaTabela
            {
                IdGrupoCorredorArmazenagem = q.GrupoCorredorArmazenagem.IdGrupoCorredorArmazenagem,
                PontoArmazenagemDescricao = q.GrupoCorredorArmazenagem.PontoArmazenagem.Descricao,
                CorredorInicial = q.GrupoCorredorArmazenagem.CorredorInicial,
                CorredorFinal = q.GrupoCorredorArmazenagem.CorredorFinal,
                IdPedidoVendaVolume = q.IdPedidoVendaVolume,
                IdPedidoVendaStatus = q.IdPedidoVendaStatus
            });

            return selectQuery.ToList();
        }

        public List<MovimentacaoVolumesDetalhesModel> BuscarDadosVolumes(DateTime dataInicial, DateTime dataFinal, long idGrupoCorredorArmazenagem, List<PedidoVendaStatusEnum> listaStatus, long idEmpresa)
        {
            dataFinal = dataFinal.Date.AddDays(1).Subtract(new TimeSpan(0, 0, 1));

            var query = Entities.PedidoVendaVolume.Where(pedidoVendaVolume => pedidoVendaVolume.PedidoVenda.IdEmpresa == idEmpresa && pedidoVendaVolume.PedidoVenda.Pedido.DataCriacao >= dataInicial && pedidoVendaVolume.PedidoVenda.Pedido.DataCriacao <= dataFinal && pedidoVendaVolume.IdGrupoCorredorArmazenagem == idGrupoCorredorArmazenagem);

            if (!listaStatus.NullOrEmpty())
            {
                query = query.Where(pedidoVendaVolume => listaStatus.Contains(pedidoVendaVolume.IdPedidoVendaStatus));
            }

            var selectQuery = query.Select(pvv => new MovimentacaoVolumesDetalhesModel
            {
                PedidoNumero = pvv.PedidoVenda.Pedido.NroPedido,
                VolumeNumero = pvv.NroVolume,
                QuantidadeProdutos = pvv.PedidoVendaProdutos.Count,
                PedidoData = pvv.PedidoVenda.Pedido.DataCriacao
            });

            return selectQuery.ToList();
        }
    }
}