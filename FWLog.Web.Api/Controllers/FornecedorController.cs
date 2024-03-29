﻿using FWLog.Data;
using FWLog.Services.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class FornecedorController : ApiBaseController
    {
        private FornecedorService _fornecedorService;
        private UnitOfWork _unitOfWork;

        public FornecedorController(UnitOfWork unitOfWork, FornecedorService fornecedorService)
        {
            _unitOfWork = unitOfWork;
            _fornecedorService = fornecedorService;
        }

        [AllowAnonymous]
        [Route("api/v1/fornecedor/integrar")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultarFornecedor()
        {
            await _fornecedorService.ConsultarFornecedor();

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/fornecedor/limpar-integracao")]
        [HttpPost]
        public async Task<IHttpActionResult> LimparIntegracao()
        {
            await _fornecedorService.LimparIntegracao();

            return ApiOk();
        }
    }
}