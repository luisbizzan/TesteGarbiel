using AutoMapper;
using ExtensionMethods.String;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.TransporteEnderecoCtx;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class TransportadoraEnderecoController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILog _log;

        public TransportadoraEnderecoController(UnitOfWork unitOfWork, ILog log)
        {
            _unitOfWork = unitOfWork;
            _log = log;
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

            var transportadoraEnderecos = _unitOfWork.TransportadoraEnderecoRepository.BuscarOsDadosParaTabela(filter, out int registrosFiltrados, out int totalRegistros);

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
            var trasportadoraEndereco = _unitOfWork.TransportadoraEnderecoRepository.GetById(id);

            var viewModel = Mapper.Map<TransportadoraEnderecoDetalhesViewModel>(trasportadoraEndereco);

            return View(viewModel);
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Expedicao.CadastrarTranportadoraEndereco)]
        public ActionResult Cadastrar()
        {
            return View();
        }

        //[HttpPost]
        //[ApplicationAuthorize(Permissions = Permissions.Separacao.CadastrarCorredorImpressora)]
        //public ActionResult Cadastrar(TransportadoraEnderecoCadastroViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        viewModel.ListaImpressora = new SelectList(
        //            _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
        //            {
        //                Value = x.Id.ToString(),
        //                Text = x.Name,
        //            }), "Value", "Text"
        //            );

        //        return View(viewModel);
        //    }

        //    try
        //    {
        //        var corredorImpressora = Mapper.Map<GrupoCorredorArmazenagem>(viewModel);

        //        corredorImpressora.IdEmpresa = IdEmpresa;

        //        _service.Cadastrar(corredorImpressora);

        //        Notify.Success("Corredor x impressora cadastrado com sucesso.");
        //    }
        //    catch (BusinessException businessException)
        //    {
        //        ModelState.AddModelError(string.Empty, businessException.Message);

        //        viewModel.ListaImpressora = new SelectList(
        //            _service.BuscarImpressoraPorEmpresa(IdEmpresa).Select(x => new SelectListItem
        //            {
        //                Value = x.Id.ToString(),
        //                Text = x.Name,
        //            }), "Value", "Text"
        //            );

        //        return View(viewModel);
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}