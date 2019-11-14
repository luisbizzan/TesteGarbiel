using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOFornecedorCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOFornecedorController : Controller
    {
        private readonly UnitOfWork _uow;

        public BOFornecedorController(UnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: Fornecedor
        public ActionResult Index()
        {
            return View();
        }

        //[ApplicationAuthorize]
        public ActionResult SearchModal()
        {
            var model = new BOFornecedorSearchModalViewModel();
            return View(model);
        }

        //[ApplicationAuthorize]
        public ActionResult SearchModalPageData(DataTableFilter<BOFornecedorSearchModalFilterViewModel> model)
        {
            List<BOFornecedorSearchModalItemViewModel> boFornecedorSearchModalItemViewModel = new List<BOFornecedorSearchModalItemViewModel>();
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var query = _uow.FornecedorRepository.GetAll().AsQueryable();

            totalRecords = query.Count();

            if (!String.IsNullOrEmpty(model.CustomFilter.Codigo))
                query = query.Where(x => !String.IsNullOrEmpty(x.Codigo) && x.Codigo.Contains(model.CustomFilter.Codigo));

            if (!string.IsNullOrEmpty(model.CustomFilter.RazaoSocial))
                query = query.Where(x => x.RazaoSocial.Contains(model.CustomFilter.RazaoSocial));

            if(!string.IsNullOrEmpty(model.CustomFilter.NomeFantasia))
                query = query.Where(x => x.NomeFantasia.Contains(model.CustomFilter.NomeFantasia));

            if (!string.IsNullOrEmpty(model.CustomFilter.CNPJ))
                query = query.Where(x => x.CNPJ.Contains(model.CustomFilter.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "")));

            foreach (var item in query)
            {
                boFornecedorSearchModalItemViewModel.Add(new BOFornecedorSearchModalItemViewModel()
                {
                    IdFornecedor = item.IdFornecedor,
                    Codigo = item.Codigo,
                    RazaoSocial = item.RazaoSocial,
                    NomeFantasia = item.NomeFantasia,
                    CNPJ = item.CNPJ.Substring(0, 2) + "." + item.CNPJ.Substring(2, 3) + "." + item.CNPJ.Substring(5, 3) + "/" + item.CNPJ.Substring(8, 4) + "-" + item.CNPJ.Substring(12, 2)
                });
            }

            totalRecordsFiltered = boFornecedorSearchModalItemViewModel.Count();

            var result = boFornecedorSearchModalItemViewModel
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