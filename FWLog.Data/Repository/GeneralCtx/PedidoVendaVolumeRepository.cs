using ExtensionMethods.List;
using FWLog.Data.Models;
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
                IdTransportadora = pedidoVendaVolume.PedidoVenda.IdTransportadora,
                TransportadoraNome = pedidoVendaVolume.PedidoVenda.Transportadora.NomeFantasia,
                CodigoEndereco = pedidoVendaVolume.EnderecoTransportadora.Codigo,
                NumeroPedido = pedidoVendaVolume.PedidoVenda.Pedido.NumeroPedido,
                NumeroVolume = pedidoVendaVolume.NroVolume,
                StatusVolume = pedidoVendaVolume.PedidoVendaStatus.Descricao
            });

            return query.OrderBy(q => q.IdTransportadora).ThenBy(q => q.NumeroPedido).ThenBy(q => q.NumeroVolume);
        }

        public List<RelatorioVolumesInstaladosTransportadoraItem> BuscarDadosVolumePorTransportadora(DataTableFilter<RelatorioVolumesInstaladosTransportadoraFiltro> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PedidoVendaVolume.AsNoTracking().Where(pvv => pvv.PedidoVenda.IdEmpresa == filtro.CustomFilter.IdEmpresa).Count();

            var query = BuscarDadosVolumePorTransportadoraQuery(filtro.CustomFilter);

            totalRecordsFiltered = query.Count();

            var response = query.Skip(filtro.Start)
                                .Take(filtro.Length);

            return response.ToList();
        }

        public List<RelatorioVolumesInstaladosTransportadoraItem> BuscarDadosVolumePorTransportadora(RelatorioVolumesInstaladosTransportadoraFiltro model)
        {
            var query = BuscarDadosVolumePorTransportadoraQuery(model);

            return query.ToList();
        }

        public List<PedidoVendaVolumeGrupoArmazenagemLinhaTabela> BuscarDadosVolumeGrupoArmazenagem(DateTime dataInicial, DateTime dataFinal, bool? cartaoCredito, bool? cartaoDebito, bool? dinheiro, bool? requisicao, long idEmpresa)
        {
            dataFinal = dataFinal.Date.AddDays(1).Subtract(new TimeSpan(0, 0, 1));

            var query = Entities.PedidoVendaVolume.Where(pedidoVendaVolume => pedidoVendaVolume.PedidoVenda.IdEmpresa == idEmpresa && pedidoVendaVolume.PedidoVenda.Pedido.DataCriacao >= dataInicial && pedidoVendaVolume.PedidoVenda.Pedido.DataCriacao <= dataFinal);

            if (cartaoCredito.HasValue)
            {
                query = query.Where(q => q.PedidoVenda.Pedido.PagamentoIsCreditoIntegracao == cartaoCredito.Value);
            }

            if (cartaoDebito.HasValue)
            {
                query = query.Where(q => q.PedidoVenda.Pedido.PagamentoIsDebitoIntegracao == cartaoDebito.Value);
            }

            if (dinheiro.HasValue)
            {
                query = query.Where(q => q.PedidoVenda.Pedido.PagamentoIsDinheiroIntegracao == dinheiro.Value);
            }

            if (requisicao.HasValue)
            {
                query = query.Where(q => q.PedidoVenda.Pedido.IsRequisicao == requisicao.Value);
            }

            var selectQuery = query.Select(q => new PedidoVendaVolumeGrupoArmazenagemLinhaTabela
            {
                IdGrupoCorredorArmazenagem = q.GrupoCorredorArmazenagem.IdGrupoCorredorArmazenagem,
                PontoArmazenagemDescricao = q.GrupoCorredorArmazenagem.PontoArmazenagem.Descricao,
                CorredorInicial = q.GrupoCorredorArmazenagem.CorredorInicial,
                CorredorFinal = q.GrupoCorredorArmazenagem.CorredorFinal,
                IdPedidoVendaVolume = q.IdPedidoVendaVolume,
                IdPedidoVendaStatus = q.IdPedidoVendaStatus,
                NumeroNotaFiscal = q.PedidoVenda.Pedido.NumeroNotaFiscal
            });

            return selectQuery.ToList();
        }

        public List<MovimentacaoVolumesDetalhesModel> BuscarDadosVolumes(DateTime dataInicial, DateTime dataFinal, long? idGrupoCorredorArmazenagem, List<PedidoVendaStatusEnum> listaStatus, bool? cartaoCredito, bool? cartaoDebito, bool? dinheiro, bool? requisicao, bool nfFaturada, long idEmpresa)
        {
            dataFinal = dataFinal.Date.AddDays(1).Subtract(new TimeSpan(0, 0, 1));

            var query = Entities.PedidoVendaVolume.Where(pedidoVendaVolume => pedidoVendaVolume.PedidoVenda.IdEmpresa == idEmpresa && pedidoVendaVolume.PedidoVenda.Pedido.DataCriacao >= dataInicial && pedidoVendaVolume.PedidoVenda.Pedido.DataCriacao <= dataFinal);

            if (idGrupoCorredorArmazenagem.HasValue)
            {
                query = query.Where(q => q.IdGrupoCorredorArmazenagem == idGrupoCorredorArmazenagem);
            }

            if (cartaoCredito.HasValue)
            {
                query = query.Where(q => q.PedidoVenda.Pedido.PagamentoIsCreditoIntegracao == cartaoCredito.Value);
            }

            if (cartaoDebito.HasValue)
            {
                query = query.Where(q => q.PedidoVenda.Pedido.PagamentoIsDebitoIntegracao == cartaoDebito.Value);
            }

            if (dinheiro.HasValue)
            {
                query = query.Where(q => q.PedidoVenda.Pedido.PagamentoIsDinheiroIntegracao == dinheiro.Value);
            }

            if (requisicao.HasValue)
            {
                query = query.Where(q => q.PedidoVenda.Pedido.IsRequisicao == requisicao.Value);
            }

            if (!listaStatus.NullOrEmpty())
            {
                query = query.Where(pedidoVendaVolume => listaStatus.Contains(pedidoVendaVolume.IdPedidoVendaStatus));
            }

            if (nfFaturada)
            {
                query = query.Where(pedidoVendaVolume => pedidoVendaVolume.PedidoVenda.Pedido.NumeroNotaFiscal.HasValue);
            }

            var selectQuery = query.Select(pvv => new MovimentacaoVolumesDetalhesModel
            {
                IdPedidoVendaVolume = pvv.IdPedidoVendaVolume,
                IdPedidoVenda = pvv.IdPedidoVenda,
                PedidoNumero = pvv.PedidoVenda.Pedido.NumeroPedido,
                VolumeNumero = pvv.NroVolume,
                QuantidadeProdutos = pvv.PedidoVendaProdutos.Count,
                PedidoData = pvv.PedidoVenda.Pedido.DataCriacao,
                VolumeCentena = pvv.NroCentena,
                TransportadoraNomeFantasia = pvv.PedidoVenda.Transportadora.NomeFantasia,
                TipoPagamentoDescricao = pvv.PedidoVenda.Pedido.PagamentoDescricaoIntegracao,
                UsuarioDespachoNotaFiscal = pvv.PedidoVenda.IdUsuarioDespachoNotaFiscal,
                DataHoraDespachoNotaFiscal = pvv.PedidoVenda.DataHoraDespachoNotaFiscal,
                UsuarioRomaneio = pvv.PedidoVenda.IdUsuarioRomaneio,
                DataHoraRomaneio = pvv.PedidoVenda.DataHoraRomaneio
            });

            var responseList = selectQuery.OrderBy(s => s.PedidoNumero).ThenBy(s => s.VolumeNumero).ToList();

            return responseList;
        }

        public List<PedidoVendaVolume> BuscarPedidosExpedidosPorEmpresa(long idEmpresa)
        {
            return Entities.PedidoVendaVolume.AsNoTracking()
                .Where(pv => pv.IdPedidoVendaStatus == PedidoVendaStatusEnum.RomaneioImpresso &&
                             pv.PedidoVenda.IdEmpresa == idEmpresa &&
                             pv.PedidoVenda.Pedido.NumeroNotaFiscal.HasValue &&
                             pv.PedidoVenda.Pedido.SerieNotaFiscal != null).ToList();
        }

        public IEnumerable<PedidoVendaVolumePesquisaModalLinhaTabela> ObterDadosParaDataTable(DataTableFilter<PedidoVendaVolumePesquisaModalFiltro> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PedidoVendaVolume.Count();

            IQueryable<PedidoVendaVolumePesquisaModalLinhaTabela> query = Entities.PedidoVendaVolume.AsNoTracking()
                .Where(x => x.PedidoVenda.Pedido.NumeroPedido == filter.CustomFilter.NroPedido &&
                (filter.CustomFilter.IdPedidoVendaVolume.HasValue == false || x.IdPedidoVendaVolume != filter.CustomFilter.IdPedidoVendaVolume.Value) &&
                (x.PedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao ||
                 x.PedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao) &&
                (x.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao || 
                 x.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao || 
                 x.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso) &&
                (filter.CustomFilter.NroVolume.HasValue == false || x.NroVolume == filter.CustomFilter.NroVolume))
                .Select(e => new PedidoVendaVolumePesquisaModalLinhaTabela
                {
                    NroPedido = e.PedidoVenda.Pedido.NumeroPedido,
                    NroVolume = e.NroVolume,
                    DescricaoStatus = e.PedidoVendaStatus.Descricao,
                    IdPedidoVendaVolume = e.IdPedidoVendaVolume
                });

            totalRecordsFiltered = query.Count();

            return query.PaginationResult(filter);
        }
    }
}