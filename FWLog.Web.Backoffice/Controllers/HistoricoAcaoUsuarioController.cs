using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Relatorio.Model;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.HistoricoAcaoUsuarioCtx;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class HistoricoAcaoUsuarioController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly RelatorioService _relatorioService;
        private readonly ILog _log;

        public HistoricoAcaoUsuarioController(UnitOfWork unitOfWork, RelatorioService relatorioService, ILog log)
        {
            _unitOfWork = unitOfWork;
            _relatorioService = relatorioService;
            _log = log;
        }

        [ApplicationAuthorize(Permissions = Permissions.HistoricoAcaoUsuario.Listar)]
        public ActionResult Index()
        {

            var model = new HistoricoAcaoUsuarioViewModel
            {
                Filter = new HistoricoDeAcoesFilterViewModel()
                {
                    ListaColetorAplicacao = new SelectList(
                    _unitOfWork.ColetorAplicacaoRepository.Todos().OrderBy(o => o.IdColetorAplicacao).Select(x => new SelectListItem
                    {
                        Value = x.IdColetorAplicacao.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"),

                    ListaHistoricoColetorTipo = new SelectList(
                    _unitOfWork.ColetorHistoricoTipoRepository.Todos().OrderBy(o => o.IdColetorHistoricoTipo).Select(x => new SelectListItem
                    {
                        Value = x.IdColetorHistoricoTipo.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
            )
                }
            };

            model.Filter.DataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(-7);
            model.Filter.DataFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(10);

            return View(model);
        }

        [ApplicationAuthorize(Permissions = Permissions.HistoricoAcaoUsuario.Listar)]
        public ActionResult PageData(DataTableFilter<HistoricoDeAcoesFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<HistoricoAcaoUsuarioFilter>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<HistoricoAcaoUsuarioLinhaTabela> result = _unitOfWork.ColetorHistoricoRepository.ObterDados(filter, out int recordsFiltered, out int totalRecords);

            result.ForEach(x => x.Usuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(x.Usuario).Nome);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<HistoricoDeAcoesListItemViewModel>>(result)
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.HistoricoAcaoUsuario.Listar)]
        public ActionResult DownloadHistorico(DownloadHistoricoAcaoUsuarioViewModel viewModel)
        {
            ValidateModel(viewModel);

            var relatorioRequest = Mapper.Map<RelatorioHistoricoAcaoUsuarioRequest>(viewModel);
            relatorioRequest.IdEmpresa = IdEmpresa;
            relatorioRequest.NomeUsuarioRequisicao = LabelUsuario;
            byte[] relatorio = _relatorioService.GerarRelatorioHistoricoAcaoUsuario(relatorioRequest);

            return File(relatorio, "application/pdf", "Relatório Resumo Atividades RF.pdf");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.HistoricoAcaoUsuario.Listar)]
        public JsonResult ImprimirHistorico(ImprimirHistoricoAcaoUsuarioViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var request = Mapper.Map<ImprimirRelatorioHistoricoAcaoUsuarioRequest>(viewModel);

                request.IdEmpresa = IdEmpresa;
                request.NomeUsuarioRequisicao = LabelUsuario;

                _relatorioService.ImprimirRelatorioHistoricoAcaoUsuario(request);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}