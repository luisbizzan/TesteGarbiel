using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.ExpedicaoCtx;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ExpedicaoController : BOBaseController
    {
        private readonly ExpedicaoService _expedicaoService;
        private readonly RelatorioService _relatorioService;

        public ExpedicaoController(ExpedicaoService expedicaoService, RelatorioService relatorioService)
        {
            _expedicaoService = expedicaoService;
            _relatorioService = relatorioService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioVolumesInstaladosTransportadora)]
        public ActionResult RelatorioVolumesInstaladosTransportadora()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioVolumesInstaladosTransportadora)]
        public ActionResult RelatorioVolumesInstaladosTransportadoraPageData(DataTableFilter<RelatorioVolumesInstaladosTransportadoraFilterViewModel> model)
        {
            var listaRetorno = new List<RelatorioVolumesInstaladosTransportadoraListItemViewModel>();

            var filtro = Mapper.Map<DataTableFilter<RelatorioVolumesInstaladosTransportadoraFiltro>>(model);

            filtro.CustomFilter.IdEmpresa = IdEmpresa;

            var volumesTransportadora = _expedicaoService.BuscarDadosVolumePorTransportadora(filtro, out int totalRecordsFiltered, out int totalRecords);

            volumesTransportadora.ForEach(volume => listaRetorno.Add(new RelatorioVolumesInstaladosTransportadoraListItemViewModel
            {
                Transportadora = volume.Transportadora,
                CodigoEndereco = volume.CodigoEndereco,
                NumeroPedido = volume.NumeroPedido.ToString(),
                NumeroVolume = volume.NumeroVolume.ToString()
            }));

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = listaRetorno
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioVolumesInstaladosTransportadora)]
        public ActionResult DownloadRelatorioVolumesInstaladosTransportadora(RelatorioVolumesInstaladosTransportadoraFilterViewModel filtro)
        {
            var requisicaoRelatorio = Mapper.Map<RelatorioVolumesInstaladosTransportadoraFiltro>(filtro);

            requisicaoRelatorio.IdEmpresa = IdEmpresa;

            var relatorio = _relatorioService.GerarRelatorioVolumesInstaladosTransportadora(requisicaoRelatorio, LabelUsuario);

            return File(relatorio, "application/pdf", "Relatório - Volumes Instalados X Transportadora.pdf");
        }

        [HttpGet]
        public ActionResult PedidoVendaPesquisaModal()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize]
        public ActionResult PedidoVendaPesquisaModalDadosLista(DataTableFilter<PedidoVendaModalViewModelFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<PedidoVendaFiltro>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            var pedidosVendaTabela = _expedicaoService.BuscarDadosPedidoVendaParaTabela(filter, out int registrosFiltrados, out int totalRegistros);

            var listaPedidosVenda = new List<PedidoVendaModalViewModelListItemViewModel>();

            pedidosVendaTabela.ForEach(pv => listaPedidosVenda.Add(new PedidoVendaModalViewModelListItemViewModel
            {
                IdPedidoVenda = pv.IdPedidoVenda,
                NumeroPedido = pv.NumeroPedido,
                NumeroPedidoVenda = pv.NumeroPedidoVenda,
                ClienteNome = pv.ClienteNome,
                TransportadoraNome = pv.TransportadoraNome,
            }));

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = listaPedidosVenda
            });
        }
    }
}