using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using FWLog.Web.Backoffice.Helpers;
using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.Models.GarantiaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Linq;
using ExtensionMethods.String;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using FWLog.Data.EnumsAndConsts;
using System.Net;
using Newtonsoft.Json;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GarantiaController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private readonly GarantiaService _garantiaService;
        private readonly ApplicationLogService _applicationLogService;
        private readonly ConferenciaService _conferenciaService;

        public GarantiaController(
            UnitOfWork uow,
            GarantiaService garantiaService,
            ApplicationLogService applicationLogService,
            ConferenciaService conferenciaService)
        {
            _uow = uow;
            _garantiaService = garantiaService;
            _applicationLogService = applicationLogService;
            _conferenciaService = conferenciaService;
        }

        public ActionResult Index()
        {
            var model = new GarantiaSolicitacaoVM
            {
                Filter = new GarantiaSolicitacaoFilterVM()
                {
                    Lista_Status = new SelectList(
                  _uow.GeralRepository.TodosTiposDaCategoria("GAR_SOLICITACAO", "ID_STATUS").OrderBy(o => o.Id).Select(x => new SelectListItem
                  {
                      Value = x.Id.ToString(),
                      Text = x.Descricao,
                  }), "Value", "Text"),
                    Lista_Tipos = new SelectList(
                  _uow.GeralRepository.TodosTiposDaCategoria("GAR_SOLICITACAO", "ID_TIPO").OrderBy(o => o.Id).Select(x => new SelectListItem
                  {
                      Value = x.Id.ToString(),
                      Text = x.Descricao,
                  }), "Value", "Text")
                }
            };

            //.Insert(0, new SelectListItem { Text = "Todos", Value = "0" })

            model.Filter.Data_Inicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(-7);
            model.Filter.Data_Final = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(10);

            return View(model);
        }

        public ActionResult VisualizarSolicitacao(long Id)
        {
            var categoria = _geralService.SelecionaUploadCategoria(Id_Categoria);

            if (categoria == null)
                return PartialView("_FormUploads", new GeralUploadVM());

            var model = new GeralUploadVM
            {
                Id_Ref = Id_Ref,
                Id_Categoria = Id_Categoria,
                Formatos = categoria.Formatos.Split(',').ToList(),
                Tabela = categoria.Tabela,
                Lista_Uploads = _geralService.TodosUploadsDaCategoria(Id_Categoria, Id_Ref)
                    .Select(x => new GeralUploadVM
                    {
                        Id = x.Id,
                        Arquivo = x.Arquivo,
                        Arquivo_Tipo = x.Arquivo_Tipo,
                        Id_Usr = x.Id_Usr,
                        Usuario = x.Usuario,
                        Dt_Cad = x.Dt_Cad
                    }).ToList(),
            };

            return PartialView("_FormUploads", model);
        }

        public ActionResult PageData(DataTableFilter<GarantiaSolicitacaoFilterVM> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<GarantiaFilter>>(model);

            //filter.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<GarSolicitacao> result = _uow.GarantiaRepository.ListarSolicitacao(filter, out recordsFiltered, out totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<GarantiaSolicitacaoListVM>>(result)
            });
        }
    }
}