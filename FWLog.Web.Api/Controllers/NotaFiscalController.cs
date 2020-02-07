﻿using FWLog.Data;
using FWLog.Services.Services;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class NotaFiscalController : ApiBaseController
    {
        private NotaFiscalService _notaFiscalService;
        private UnitOfWork _unitOfWork;

        public NotaFiscalController(UnitOfWork unitOfWork, NotaFiscalService notaFiscalService)
        {
            _unitOfWork = unitOfWork;
            _notaFiscalService = notaFiscalService;
        }

        [Route("api/v1/nota-fiscal/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultaNota()
        {
            await _notaFiscalService.ConsultaNotaFiscalCompra();

            return ApiOk();
        }

        [Route("api/v1/nota-fiscal/receber/automatico")]
        [HttpPost]
        public async Task<IHttpActionResult> ReceberNotaFiscalAutomatico()
        {
            await _notaFiscalService.ReceberNotaFiscalAutomatico(User.Identity.GetUserId());

            return ApiOk();
        }
    }
}