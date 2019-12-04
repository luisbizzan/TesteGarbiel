using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOEmpresaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOEmpresaController : BOBaseController
    {
        private readonly UnitOfWork _uow;

        public BOEmpresaController(UnitOfWork uow)
        {
            _uow = uow;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscarEmpresas()
        {
            ViewData["Empresas"] = new SelectList(Empresas, "IdEmpresa", "Nome");
            return PartialView("_MudarEmpresa");
        }

        public JsonResult MudarEmpresa(int idEmpresa)
        {
            var userInfo = new BackOfficeUserInfo();
            CookieSalvarEmpresa(idEmpresa, userInfo.UserId.ToString());

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = Resources.CommonStrings.ChangeCompanySuccess
            }, JsonRequestBehavior.DenyGet);
        }

        public long BuscarIdEmpresa()
        {
            return IdEmpresa;
        }

        public ActionResult SearchModal(string campoSelecionado)
        {
            var model = new BOEmpresaSearchModalViewModel(campoSelecionado);
            return View(model);
        }

        public ActionResult SearchModalPageData(DataTableFilter<BOEmpresaSearchModalFilterViewModel> model)
        {
            List<BOEmpresaSearchModalItemViewModel> boEmpresaSearchModalItemViewModel = new List<BOEmpresaSearchModalItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var query = _uow.EmpresaConfigRepository.Todos();

            totalRecords = query.Count();

            if (model.CustomFilter.CodigoIntegracao.HasValue)
            {
                query = query.Where(x => x.Empresa.CodigoIntegracao == model.CustomFilter.CodigoIntegracao.Value);
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.RazaoSocial))
            {
                query = query.Where(x => x.Empresa.RazaoSocial.Contains(model.CustomFilter.RazaoSocial));
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.NomeFantasia))
            {
                query = query.Where(x => x.Empresa.NomeFantasia.Contains(model.CustomFilter.NomeFantasia));
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.Sigla))
            {
                query = query.Where(x => x.Empresa.Sigla == model.CustomFilter.Sigla);
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.CNPJ))
            {
                query = query.Where(x => x.Empresa.CNPJ.Contains(model.CustomFilter.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "")));
            }

            if (!string.IsNullOrEmpty(model.CustomFilter.CampoSelecionado) && model.CustomFilter.CampoSelecionado.Contains("Matriz"))
            {
                query = query.Where(x => x.IdEmpresaTipo != EmpresaTipoEnum.Filial);
            }
                                   
            foreach (var item in query)
            {
                boEmpresaSearchModalItemViewModel.Add(new BOEmpresaSearchModalItemViewModel()
                {
                    IdEmpresa = item.IdEmpresa,
                    CodigoIntegracao = item.Empresa.CodigoIntegracao,
                    RazaoSocial = item.Empresa.RazaoSocial,
                    NomeFantasia = item.Empresa.NomeFantasia,
                    Sigla = item.Empresa.Sigla,
                    CNPJ = item.Empresa.CNPJ.Substring(0, 2) + "." + item.Empresa.CNPJ.Substring(2, 3) + "." + item.Empresa.CNPJ.Substring(5, 3) + "/" + item.Empresa.CNPJ.Substring(8, 4) + "-" + item.Empresa.CNPJ.Substring(12, 2) 
                });
            }

            totalRecordsFiltered = boEmpresaSearchModalItemViewModel.Count();

            var result = boEmpresaSearchModalItemViewModel
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = result
            });
        }
    }
}