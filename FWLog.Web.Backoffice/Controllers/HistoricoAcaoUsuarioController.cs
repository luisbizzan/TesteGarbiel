using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOAccountCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class HistoricoAcaoUsuarioController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public HistoricoAcaoUsuarioController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ApplicationAuthorize(Permissions = Permissions.HistoricoAcaoUsuario.Listar)]
        public ActionResult Index()
        {

            var model = new HistoricoDeAcoesViewModel
            {
                Filter = new HistoricoDeAcoesFilterViewModel()
                {
                    ListaColetorAplicacao = new SelectList(
                    _unitOfWork.ColetorAplicacaoRepository.Todos().OrderBy(o => o.IdColetorAplicacao).Select(x => new SelectListItem
                    {
                        Value = x.IdColetorAplicacao.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"),

                    ListaHistoricoColetorTipo = new SelectList(
                    _unitOfWork.ColetorHistoricoTipoRepository.Todos().OrderBy(o => o.IdColetorHistoricoTipo).Select(x => new SelectListItem
                    {
                        Value = x.IdColetorHistoricoTipo.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
            )
                }
            };

            model.Filter.DataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(-7);
            model.Filter.DataFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(10);

            return View(model);
        }

        [ApplicationAuthorize(Permissions = Permissions.HistoricoAcaoUsuario.Listar)]
        public ActionResult PageData(DataTableFilter<HistoricoDeAcoesFilterViewModel> model)
        {
            var filter = Mapper.Map<DataTableFilter<HistoricoAcaoUsuarioFilter>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<HistoricoAcaoUsuarioLinhaTabela> result = _unitOfWork.ColetorHistoricoRepository.ObterDadosParaDataTable(filter, out int recordsFiltered, out int totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<HistoricoDeAcoesListItemViewModel>>(result)
            });
        }
    }
}