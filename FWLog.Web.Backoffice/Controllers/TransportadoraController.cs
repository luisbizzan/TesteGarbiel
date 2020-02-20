using AutoMapper;
using ExtensionMethods.String;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.TransportadoraCtx;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class TransportadoraController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public TransportadoraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private void setViewBags()
        {
            ViewBag.Status = new SelectList(new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Ativo", Value = "true"},
                            new SelectListItem { Text = "Inativo", Value = "false"}
                        }, "Value", "Text");
        }

        [HttpGet]
        public ActionResult SearchModal()
        {
            setViewBags();

            var model = new TransportadoraSearchModalViewModel();
            return View(model);
        }


        [HttpPost]
        public ActionResult SearchModalPageData(DataTableFilter<TransportadoraPesquisaModalFiltro> model)
        {
            IEnumerable<TransportadoraPesquisaModalLinhaTabela> result = _unitOfWork.TransportadoraRepository.ObterDadosParaDataTable(model, out int recordsFiltered, out int totalRecords);

            //Formatando o campo CNPJ para o datatable
            result.ForEach(x => x.CNPJ = x.CNPJ.CnpjOuCpf());

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<TransportadoraSearchModalItemViewModel>>(result)
            });
        }
    }
}