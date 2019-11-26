using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.PontoArmazenagemCtx;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PontoArmazenagemController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public PontoArmazenagemController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.PontoArmazenagem.Listar)]
        public ActionResult Index()
        {
            var viewModel = new PontoArmazenagemListaViewModel
            {
                NiveisArmazenagem = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Todos", Value = ""}
                }, "Value", "Text"),
                TiposArmazenagem = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Todos", Value = ""}
                }, "Value", "Text"),
                TiposMovimentacao = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Todos", Value = ""}
                }, "Value", "Text"),
                Status = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Todos", Value = ""},
                    new SelectListItem { Text = "Ativo", Value = "1"},
                    new SelectListItem { Text = "Inativo", Value = "2"}
                }, "Value", "Text")
            };

            return View(viewModel);
        }
    }
}