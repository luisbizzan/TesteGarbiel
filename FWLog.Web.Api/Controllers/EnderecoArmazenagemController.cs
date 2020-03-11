using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Web.Api.Models.EnderecoArmazenagem;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class EnderecoArmazenagemController : ApiBaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public EnderecoArmazenagemController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("api/v1/enderecos")]
        public IHttpActionResult ImprimirEtiquetaEndereco([FromUri] string identificacao)
        {
            if (string.IsNullOrWhiteSpace(identificacao))
            {
                return ApiBadRequest($"{nameof(identificacao)} cannot be null.");
            }

            _ = long.TryParse(identificacao, out long _identificacao);

            var lista = _unitOfWork.EnderecoArmazenagemRepository.Tabela().Where(x => x.IdEmpresa == IdEmpresa && x.Ativo && (x.Codigo == identificacao || x.IdEnderecoArmazenagem == _identificacao))
                .Select(x => new EnderecoArmazenagemResposta
                {
                    Codigo = x.Codigo,
                    IdEnderecoArmazenagem = x.IdEnderecoArmazenagem,
                    DescricaoNivel = x.NivelArmazenagem.Descricao,
                    DescricaoPonto = x.PontoArmazenagem.Descricao
                }).ToList();

            var rtn = new EnderecosArmazenagemResposta { Lista = lista };

            return ApiOk(rtn);
        }

        [HttpGet]
        [Route("api/v1/endereco/{id}")]
        public async Task<IHttpActionResult> Pesquisar(long id)
        {
            if (id <= 0)
            {
                return ApiBadRequest("O endereço deve ser informado.");
            }

            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(id);

            if (enderecoArmazenagem == null)
            {
                return ApiNotFound("O endereço não foi encontrado.");
            }

            var enderecoArmazenagemResposta = Mapper.Map<PesquisarEnderecoArmazenagemPorIdResposta>(enderecoArmazenagem);

            return ApiOk(enderecoArmazenagemResposta);
        }
    }
}