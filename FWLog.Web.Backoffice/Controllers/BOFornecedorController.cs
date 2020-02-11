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

            var query = _uow.FornecedorRepository.Todos();

            totalRecords = query.Count();

            if (model.CustomFilter.IdFornecedor.HasValue)
                query = query.Where(x => x.IdFornecedor == model.CustomFilter.IdFornecedor.Value);

            if (!string.IsNullOrEmpty(model.CustomFilter.RazaoSocial))
                query = query.Where(x => x.RazaoSocial.Contains(model.CustomFilter.RazaoSocial));

            if (!string.IsNullOrEmpty(model.CustomFilter.NomeFantasia))
                query = query.Where(x => x.NomeFantasia.Contains(model.CustomFilter.NomeFantasia));

            if (!string.IsNullOrEmpty(model.CustomFilter.CNPJ))
                query = query.Where(x => x.CNPJ.Contains(model.CustomFilter.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "")));

            foreach (var item in query)
            {
                boFornecedorSearchModalItemViewModel.Add(new BOFornecedorSearchModalItemViewModel()
                {
                    IdFornecedor = item.IdFornecedor,
                    CodigoIntegracao = item.CodigoIntegracao,
                    RazaoSocial = item.RazaoSocial,
                    NomeFantasia = item.NomeFantasia,
                    CNPJ = item.CNPJ.Length == 14 ? item.CNPJ.Substring(0, 2) + "." + item.CNPJ.Substring(2, 3) + "." + item.CNPJ.Substring(5, 3) + "/" + item.CNPJ.Substring(8, 4) + "-" + item.CNPJ.Substring(12, 2) : item.CNPJ
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