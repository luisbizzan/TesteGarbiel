using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Armazenagem;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class HistoricoColetorController : ApiBaseController
    {
        private ColetorHistoricoService _coletorHistoricoService;

        public HistoricoColetorController(ColetorHistoricoService coletorHistoricoService)
        {
            _coletorHistoricoService = coletorHistoricoService;
        }

        [Route("api/v1/historico/gravar")]
        [HttpPost]
        public IHttpActionResult GravarHistorico(GravarHistoricoRequisicao historico)
        {
            var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = (ColetorAplicacaoEnum)historico.IdColetorAplicacao,
                IdColetorHistoricoTipo = (ColetorHistoricoTipoEnum)historico.IdColetorHistoricoTipo,
                Descricao = historico.Descricao,
                IdEmpresa = IdEmpresa,
                IdUsuario = IdUsuario
            };

            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);

            return ApiOk();
        }
    }
}