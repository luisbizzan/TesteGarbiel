using FWLog.Data;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Etiqueta;
using System;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class EtiquetaController : ApiBaseController
    {
        private readonly EtiquetaService _etiquetaService;
        private readonly UnitOfWork _unitOfWork;

        public EtiquetaController(UnitOfWork unitOfWork, EtiquetaService etiquetaService)
        {
            _unitOfWork = unitOfWork;
            _etiquetaService = etiquetaService;
        }

        [HttpPost]
        [Route("api/v1/etiqueta/endereco")]
        public IHttpActionResult ImprimirEtiquetaEndereco(ImprimirEtiquetaEnderecoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var requisicaoServico = new ImprimirEtiquetaEnderecoRequest
                {
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    IdImpressora = requisicao.IdImpressora
                };

                _etiquetaService.ImprimirEtiquetaEndereco(requisicaoServico);

                return ApiOk();
            }
            catch(BusinessException ex)
            {
                return ApiInternalServerErrror("Erro na impressora", ex);
            }
            catch
            {
                throw;
            }
        }
    }
}