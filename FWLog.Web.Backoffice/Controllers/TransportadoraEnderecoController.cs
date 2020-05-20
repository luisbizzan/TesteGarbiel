using AutoMapper;
using DartDigital.Library.Exceptions;
using ExtensionMethods.String;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.TransporteEnderecoCtx;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class TransportadoraEnderecoController : BOBaseController
    {
        private readonly ILog _log;
        private readonly TransportadoraEnderecoService _transportadoraEnderecoService;
        private readonly UnitOfWork _unitOfWork;

        public TransportadoraEnderecoController(ILog log, TransportadoraEnderecoService transportadoraEnderecoService, UnitOfWork unitOfWork)
        {
            _log = log;
            _transportadoraEnderecoService = transportadoraEnderecoService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.ListarTranportadoraEndereco)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.ListarTranportadoraEndereco)]
        public ActionResult PageData(DataTableFilter<TransporteEnderecoListaFiltroViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<TransportadoraEnderecoListaFiltro>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            var transportadoraEnderecos = _transportadoraEnderecoService.BuscarDadosParaTabela(filter, out int registrosFiltrados, out int totalRegistros);

            var list = new List<TransporteEnderecoListaItemViewModel>();

            transportadoraEnderecos.OrderBy(x => x.IdTransportadora).ThenBy(x => x.Codigo).ForEach(te => list.Add(new TransporteEnderecoListaItemViewModel
            {
                DadosTransportadora = string.Concat(te.CnpjTransportadora.CnpjOuCpf(), " - ", te.RazaoSocialTransportadora),
                Codigo = te.Codigo,
                IdTransportadoraEndereco = te.IdTransportadoraEndereco
            }));

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.VisualizarTranportadoraEndereco)]
        public ActionResult Detalhes(long id)
        {
            var trasportadoraEndereco = _transportadoraEnderecoService.BuscarTransportadoraEndereco(id);

            var viewModel = Mapper.Map<TransportadoraEnderecoDetalhesViewModel>(trasportadoraEndereco);

            return View(viewModel);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.CadastrarTranportadoraEndereco)]
        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.CadastrarTranportadoraEndereco)]
        public ActionResult Cadastrar(TransportadoraEnderecoCadastroViewModel viewModel)
        {
            ValidateModel(viewModel);

            try
            {
                var transportadoraEndereco = Mapper.Map<TransportadoraEndereco>(viewModel);

                _transportadoraEnderecoService.Cadastrar(transportadoraEndereco, IdEmpresa);

                Notify.Success("Transportadora x Endereço cadastrado com sucesso.");
            }
            catch (BusinessException businessException)
            {
                ModelState.AddModelError(string.Empty, businessException.Message);

                return View(viewModel);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);

                Notify.Error("Ocorreu um erro ao vincular endereço na transportadora.");

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.EditarTranportadoraEndereco)]
        public ActionResult Editar(long id)
        {
            var transportadoraEndereco = _transportadoraEnderecoService.BuscarTransportadoraEndereco(id);

            var viewModel = Mapper.Map<TransportadoraEnderecoEdicaoViewModel>(transportadoraEndereco);

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.EditarTranportadoraEndereco)]
        public ActionResult Editar(TransportadoraEnderecoEdicaoViewModel viewModel)
        {
            ValidateModel(viewModel);

            try
            {
                var transportadoraEndereco = Mapper.Map<TransportadoraEndereco>(viewModel);

                _transportadoraEnderecoService.Editar(transportadoraEndereco, IdEmpresa);

                Notify.Success("Transportadora x Endereço editado com sucesso.");
            }
            catch (BusinessException businessException)
            {
                ModelState.AddModelError(string.Empty, businessException.Message);

                return View(viewModel);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);

                Notify.Error("Ocorreu um erro ao vincular endereço na transportadora.");

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.ExcluirTranportadoraEndereco)]
        public JsonResult ExcluirAjax(int id)
        {
            try
            {
                _transportadoraEnderecoService.Excluir(id);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
                }, JsonRequestBehavior.DenyGet);
            }
            catch (BusinessException exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = exception.Message
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public ActionResult PesquisaModal()
        {
            var model = new TransporteEnderecoListaViewModel()
            {
                Filtros = new TransporteEnderecoListaFiltroViewModel()
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize]
        public ActionResult PesquisaModalDadosLista(DataTableFilter<TransporteEnderecoListaFiltroViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<TransportadoraEnderecoListaFiltro>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            if (!filter.CustomFilter.CodigoEnderecoArmazenagem.NullOrEmpty())
            {
                var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarPorCodigo(filter.CustomFilter.CodigoEnderecoArmazenagem, IdEmpresa).FirstOrDefault();

                filter.CustomFilter.IdEnderecoArmazenagem = (enderecoArmazenagem?.IdEnderecoArmazenagem).GetValueOrDefault();
            }

            var transportadoraEnderecos = _transportadoraEnderecoService.BuscarDadosParaTabela(filter, out int registrosFiltrados, out int totalRegistros);

            var list = new List<TransporteEnderecoListaItemViewModel>();

            transportadoraEnderecos.OrderBy(x => x.IdTransportadora).ThenBy(x => x.Codigo).ForEach(te => list.Add(new TransporteEnderecoListaItemViewModel
            {
                DadosTransportadora = string.Concat(te.CnpjTransportadora.CnpjOuCpf(), " - ", te.RazaoSocialTransportadora),
                Codigo = te.Codigo,
                IdTransportadoraEndereco = te.IdTransportadoraEndereco
            }));

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = list
            });
        }
    }
}