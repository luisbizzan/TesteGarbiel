using AutoMapper;
using ExtensionMethods.String;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.PedidoVendaVolumeCtx;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PedidoVendaVolumeController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public PedidoVendaVolumeController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult SearchModal()
        {
            return View(new PedidoVendaVolumeSearchModalViewModel());
        }

        [HttpPost]
        public ActionResult SearchModalPageData(DataTableFilter<PedidoVendaVolumePesquisaModalFiltro> model)
        {
            IEnumerable<PedidoVendaVolumePesquisaModalLinhaTabela> result = _unitOfWork.PedidoVendaVolumeRepository.ObterDadosParaDataTable(model, out int recordsFiltered, out int totalRecords);
            
            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<PedidoVendaVolumeSearchModalItemViewModel>>(result)
            });
        }
    }
}