﻿using FWLog.Data;
using FWLog.Web.Api.Models.EnderecoArmazenagem;
using System.Linq;
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
    }
}