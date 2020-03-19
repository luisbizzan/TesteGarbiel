using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Web.Api.Models.EnderecoArmazenagem;
using System.Collections.Generic;
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
        [Route("api/v1/endereco/pesquisar/")]
        public IHttpActionResult Perquisar(string idCod)
        {
            if (string.IsNullOrEmpty(idCod))
            {
                return ApiBadRequest("Informe o código de barras ou o código do endereço.");
            }

            var resposta = new EnderecosArmazenagemResposta { Lista = new List<EnderecoArmazenagemResposta>() };

            if(long.TryParse(idCod, out long idEnderecoArmazenagem))
            {
                EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

                if (enderecoArmazenagem != null)
                {
                    var itemResposta = new EnderecoArmazenagemResposta
                    {
                        Codigo = enderecoArmazenagem.Codigo,
                        IdEnderecoArmazenagem = enderecoArmazenagem.IdEnderecoArmazenagem,
                        DescricaoNivel = enderecoArmazenagem.NivelArmazenagem.Descricao,
                        DescricaoPonto = enderecoArmazenagem.PontoArmazenagem.Descricao,
                        IsPontoSeparacao = enderecoArmazenagem.IsPontoSeparacao
                    };

                    resposta.Lista.Add(itemResposta);

                    return ApiOk(resposta);
                }
            }

            List<EnderecoArmazenagem> enderecosArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarPorCodigo(idCod, IdEmpresa);

            if (enderecosArmazenagem.Count == 0)
            {
                return ApiNotFound("Nenhum endereço foi encontrado.");
            }

            foreach (EnderecoArmazenagem enderecoArmazenagem in enderecosArmazenagem)
            {
                var itemResposta = new EnderecoArmazenagemResposta
                {
                    Codigo = enderecoArmazenagem.Codigo,
                    IdEnderecoArmazenagem = enderecoArmazenagem.IdEnderecoArmazenagem,
                    DescricaoNivel = enderecoArmazenagem.NivelArmazenagem.Descricao,
                    DescricaoPonto = enderecoArmazenagem.PontoArmazenagem.Descricao,
                    IsPontoSeparacao = enderecoArmazenagem.IsPontoSeparacao
                };

                resposta.Lista.Add(itemResposta);
            }

            return ApiOk(resposta);
        }

        [HttpGet]
        [Route("api/v1/endereco/{id}")]
        public IHttpActionResult PesquisarPorId(long id)
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