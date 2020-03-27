using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOAccountCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        [ApplicationAuthorize(Permissions = Permissions.Garantia.Listar)]
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

            //model.Filter.IdGarantiaStatus = GarantiaStatusEnum.AguardandoRecebimento.GetHashCode();
            model.Filter.DataFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(-7);
            model.Filter.DataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(10);

            return View(model);
        }
    }
}