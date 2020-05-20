using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.ExpedicaoCtx;
using log4net;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ExpedicaoController : BOBaseController
    {
        private readonly ExpedicaoService _expedicaoService;
        private readonly RelatorioService _relatorioService;
        private readonly ILog _log;

        public ExpedicaoController(ExpedicaoService expedicaoService, RelatorioService relatorioService, ILog log)
        {
            _expedicaoService = expedicaoService;
            _relatorioService = relatorioService;
            _log = log;
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

            //if (!model.CustomFilter.IdTransportadora.HasValue && !model.CustomFilter.IdPedidoVenda.HasValue && !model.CustomFilter.IdProduto.HasValue)
            //{
            //    return DataTableResult.FromModel(new DataTableResponseModel
            //    {
            //        Draw = model.Draw,
            //        RecordsTotal = 0,
            //        RecordsFiltered = 0,
            //        Data = listaRetorno
            //    });
            //}

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

        //[HttpPost]
        //[ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioVolumesInstaladosTransportadora)]
        //public ActionResult DownloadRelatorioVolumesInstaladosTransportadora(DownloadRelatorioVolumesInstaladosTransportadoraViewModel viewModel)
        //{
        //    var relatorioRequest = Mapper.Map<RelatorioVolumesInstaladosTransportadoraRequest>(viewModel);
        //    relatorioRequest.IdEmpresa = IdEmpresa;
        //    relatorioRequest.NomeUsuarioRequisicao = LabelUsuario;
        //    byte[] relatorio = _relatorioService.GerarRelatorioPosicaoParaInventario(relatorioRequest);

        //    return File(relatorio, "application/pdf", "Relatório - Posição para Inventário.pdf");
        //}

        //[HttpPost]
        //[ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioVolumesInstaladosTransportadora)]
        //public JsonResult ImprimirRelatorioVolumesInstaladosTransportadora(ImprimirRelatorioVolumesInstaladosTransportadoraViewModel viewModel)
        //{
        //    try
        //    {
        //        ValidateModel(viewModel);

        //        var request = Mapper.Map<ImprimirRelatorioVolumesInstaladosTransportadoraRequest>(viewModel);

        //        request.IdEmpresa = IdEmpresa;
        //        request.NomeUsuarioRequisicao = LabelUsuario;

        //        _relatorioService.ImprimirRelatorioPosicaoParaInventario(request);

        //        return Json(new AjaxGenericResultModel
        //        {
        //            Success = true,
        //            Message = "Impressão enviada com sucesso."
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        _log.Error(e.Message, e);

        //        return Json(new AjaxGenericResultModel
        //        {
        //            Success = false,
        //            Message = "Ocorreu um erro na impressão."
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}