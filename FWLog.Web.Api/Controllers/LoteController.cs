using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Lote;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class LoteController : ApiBaseController
    {
        private LoteService _loteService;
        private UnitOfWork _unitOfWork;
        public LoteController(UnitOfWork unitOfWork, LoteService loteService)
        {
            _unitOfWork = unitOfWork;
            _loteService = loteService;
        }

        [Route("api/v1/lote/conferir")]
        [HttpPost]
        public async Task<IHttpActionResult> ConsultaNota()
        {
            await _loteService.ConferirLoteAutomatico(User.Identity.GetUserId());

            return ApiOk();
        }

        [Route("api/v1/lote/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> BuscarPorId(long id)
        {
            if(id == 0)
            {
                return ApiBadRequest("O lote deve ser informado.");
            }

            Lote lote = _unitOfWork.LoteRepository.GetById(id);

            if(lote == null)
            {
                return ApiNotFound("O lote não foi encontrado");
            }

            var loteResposta = Mapper.Map<BuscarLotePorIdResposta>(lote);

            return ApiOk(loteResposta);
        }
    }
}