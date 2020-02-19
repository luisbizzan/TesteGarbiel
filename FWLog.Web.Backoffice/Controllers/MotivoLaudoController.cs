using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.MotivoLaudoCtx;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class MotivoLaudoController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public MotivoLaudoController(UnitOfWork unitOfWork)
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
        [ApplicationAuthorize(Permissions = Permissions.MotivoLaudo.Listar)]
        public ActionResult MotivoLaudo()
        {
            setViewBags();

            return View(new MotivoLaudoListViewModel());
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.MotivoLaudo.Listar)]
        public ActionResult PageData(DataTableFilter<MotivoLaudoFilterViewModel> model)
        {
            List<MotivoLaudoListItemViewModel> motivoLaudoListItem = new List<MotivoLaudoListItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var result = _unitOfWork.MotivoLaudoRepository.ObterTodos();

            totalRecords = result.Count();

            if (model.CustomFilter.IdMotivoLaudo.HasValue)
            {
                result = result.Where(x => x.IdMotivoLaudo == model.CustomFilter.IdMotivoLaudo);
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.Descricao))
            {
                result = result.Where(x => x.Descricao == model.CustomFilter.Descricao);
            }

            if (model.CustomFilter.Status.HasValue)
            {
                result = result.Where(x => x.Ativo == model.CustomFilter.Status.Value);
            }


            foreach (var item in result)
            {
                motivoLaudoListItem.Add(new MotivoLaudoListItemViewModel
                {
                    IdMotivoLaudo = item.IdMotivoLaudo,
                    Descricao = item.Descricao,
                    Status = item.Ativo ? "Ativo" : "Inativo",
                });
            }

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = motivoLaudoListItem
            });
        }
    }
}