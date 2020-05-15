using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.GarantiaConfiguracaoCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GarantiaEtiquetaController : BOBaseController
    {
        #region [Variáveis Sessão]
        private readonly UnitOfWork _unitOfWork;
        private readonly GarantiaEtiquetaService _garantiaEtiquetaService;

        public GarantiaEtiquetaController(UnitOfWork unitOfWork, GarantiaEtiquetaService garantiaEtiquetaService)
        {
            _unitOfWork = unitOfWork;
            _garantiaEtiquetaService = garantiaEtiquetaService;
        }
        #endregion

        #region Views
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}