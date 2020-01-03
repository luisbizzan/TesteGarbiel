using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.RecebimentoEtiquetaIndividualPersonalizadaCtx;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class RecebimentoEtiquetaIndividualPersonalizadaController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EtiquetaService _etiquetaService;
        private readonly ApplicationLogService _applicationLogService;

        public RecebimentoEtiquetaIndividualPersonalizadaController(UnitOfWork unitOfWork, EtiquetaService etiquetaService, ApplicationLogService applicationLogService)
        {
            _unitOfWork = unitOfWork;
            _etiquetaService = etiquetaService;
            _applicationLogService = applicationLogService;
        }

        public ActionResult Index()
        {
            var model = new RecebimentoEtiquetaIndividualPersonalizadaListViewModel
            {
                Filter = new RecebimentoEtiquetaIndividualPersonalizadaFilterViewModel() { }
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize]
        public ActionResult PageData(DataTableFilter<RecebimentoEtiquetaIndividualPersonalizadaFilterViewModel> model)
        {
            var filtros = Mapper.Map<DataTableFilter<LogEtiquetagemListaFiltro>>(model);

            IEnumerable<LogEtiquetagemListaLinhaTabela> result = _unitOfWork.LogEtiquetagemRepository.BuscarLista(filtros, out int registrosFiltrados, out int totalRegistros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRegistros,
                RecordsFiltered = registrosFiltrados,
                Data = Mapper.Map<IEnumerable<RecebimentoEtiquetaIndividualPersonalizadaListItemViewModel>>(result)
            });
        }
    }
}