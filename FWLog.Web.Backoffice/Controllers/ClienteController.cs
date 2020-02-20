using AutoMapper;
using ExtensionMethods.String;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.ClienteCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ClienteController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public ClienteController(UnitOfWork unitOfWork)
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

            var model = new ClienteSearchModalViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult SearchModalPageData(DataTableFilter<ClientePesquisaModalFiltro> model)
        {
            IEnumerable<ClientePesquisaModalLinhaTabela> result = _unitOfWork.ClienteRepository.ObterDadosParaDataTable(model, out int recordsFiltered, out int totalRecords);
            
            //Formatando a informação de cnpj ou cpf para o datatable
            result.ForEach(x => x.CNPJCPF = x.CNPJCPF.CnpjOuCpf());

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<ClienteSearchModalItemViewModel>>(result)
            });
        }
    }
}