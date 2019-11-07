using FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx;
using System;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    [Route("recebimento/notas")]
    public class BORecebimentoNotaController : BOBaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DetalhesEtiquetaConferencia()
        {
            var viewModel = new BODetalhesEtiquetaConferenciaViewModel
            {
                NumeroNotaFiscal = "42-10/04-84.684.182/0001-57-55-001-000.000.002-010.804.210-8",
                DataHoraRecebimento = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                NomeFornecedor = "Nome do Fornecedor Nota Fiscal",
                QuantidadeVolumes = "05"
            };

            return View(viewModel);
        }
    }
}