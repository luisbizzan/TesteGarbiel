using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Caixa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class CaixaController : ApiBaseController
    {
        private readonly CaixaService _caixaService;

        public CaixaController(CaixaService caixaService)
        {
            _caixaService = caixaService;
        }

        [Route("api/v1/caixa/pesquisar-tipo-separacao")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult PesquisarTipoSeparacao()
        {
            try
            {
                _caixaService.ValidarPesquisa(219);
                
                var caixasDeSeparacao = _caixaService.BuscarCaixaTipoSeparacao(219);

                var response = Mapper.Map<List<CaixaResposta>>(caixasDeSeparacao);

                return ApiOk(response);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
            catch 
            {
                throw;
            }
        }
    }
}
