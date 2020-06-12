using DartDigital.Library.Exceptions;
using FWLog.Services.Model.Armazenagem;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Armazenagem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class ArmazenagemController : ApiBaseController
    {
        private readonly ArmazenagemService _armazenagemService;

        public ArmazenagemController(ArmazenagemService armazenagemService)
        {
            _armazenagemService = armazenagemService;
        }

        [Route("api/v1/armazenagem/instalar/validar-lote/{idLote}")]
        [HttpGet]
        public IHttpActionResult ValidarLoteInstalacao(long idLote)
        {
            try
            {
                var validarLoteRequisicao = new ValidarLoteRequisicao
                {
                    IdLote = idLote,
                    IdEmpresa = IdEmpresa
                };

                _armazenagemService.ValidarLote(validarLoteRequisicao);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/instalar/validar-lote-produto")]
        [HttpPost]
        public IHttpActionResult ValidarLoteProdutoInstalacao(ValidarLoteProdutoInstalacaoModelRequisicao requisicao)
        {
            try
            {
                var validarProdutoRequisicao = new ValidarLoteProdutoInstalacaoRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    IdEmpresa = IdEmpresa
                };

                _armazenagemService.ValidarLoteProdutoInstalacao(validarProdutoRequisicao);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
            catch
            {
                throw;
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/instalar/validar-quantidade")]
        [HttpPost]
        public IHttpActionResult ValidarQuantidadeInstalacao(ValidarQuantidadeInstalacaoModelRequisicao requisicao)
        {
            try
            {
                var validarQuantidadeRequisicao = new ValidarQuantidadeInstalacaoRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    Quantidade = requisicao.Quantidade,
                    IdEmpresa = IdEmpresa
                };

                _armazenagemService.ValidarQuantidadeInstalacao(validarQuantidadeRequisicao);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
            catch
            {
                throw;
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/instalar/validar-endereco")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoInstalacao(ValidarEnderecoInstalacaoModelRequisicao requisicao)
        {
            try
            {
                var validarEnderecoRequisicao = new ValidarEnderecoInstalacaoRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    Quantidade = requisicao.Quantidade,
                    IdEmpresa = IdEmpresa,
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem
                };

                var temOutrosProdutosInstalados = _armazenagemService.ValidarEnderecoInstalacao(validarEnderecoRequisicao);

                return ApiOk(new ValidarEnderecoInstalacaoModelResposta
                {
                    TemOutrosProdutosInstalados = temOutrosProdutosInstalados
                });
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

        }

        [Route("api/v1/armazenagem/instalar")]
        [HttpPost]
        public async Task<IHttpActionResult> InstalarVolumeLote(InstalarVolumeLoteModelRequisicao requisicao)
        {
            try
            {
                var instalarVolumeLoteRequisicao = new InstalarVolumeLoteRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    Quantidade = requisicao.Quantidade,
                    IdEmpresa = IdEmpresa,
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    IdUsuarioInstalacao = IdUsuario,
                    QuantidadeCaixas = requisicao.QuantidadeCaixas
                };

                await _armazenagemService.InstalarVolumeLote(instalarVolumeLoteRequisicao);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/retirar/validar-endereco/{idEndereco}")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoRetirar(long idEndereco)
        {
            try
            {
                _armazenagemService.ValidarEndereco(idEndereco);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/retirar/validar-lote")]
        [HttpPost]
        public IHttpActionResult ValidateLoteRetirar(ValidateLoteRetirarModelRequisicao requisicao)
        {
            try
            {
                _armazenagemService.ValidateLoteRetirar(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdLote ?? 0);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/retirar/validar-produto")]
        [HttpPost]
        public IHttpActionResult ValidarProdutoRetirar(ValidarProdutoRetirarModelRequisicao requisicao)
        {
            try
            {
                _armazenagemService.ValidarProdutoRetirar(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdLote ?? 0, requisicao?.IdProduto ?? 0, IdEmpresa);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/detalhes/{idLote}/{idProduto}/{idEnderecoArmazenagem}")]
        [HttpGet]
        public IHttpActionResult ConsultaDetalhesVolumeInformado(long idLote, long idProduto, long idEnderecoArmazenagem)
        {
            try
            {
                var detalhesVolumeInformado = _armazenagemService.ConsultaDetalhesVolumeInformado(idEnderecoArmazenagem, idLote, idProduto, IdEmpresa);

                var response = new ConsultaDetalhesVolumeInformadoResposta
                {
                    Quantidade = detalhesVolumeInformado.Quantidade,
                    PesoTotal = detalhesVolumeInformado.PesoTotal,
                    LimitePeso = detalhesVolumeInformado.EnderecoArmazenagem.LimitePeso,
                    DataHoraInstalacao = detalhesVolumeInformado.DataHoraInstalacao,
                    CodigoUsuarioInstalacao = detalhesVolumeInformado.AspNetUsers.UserName
                };

                return ApiOk(response);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [Route("api/v1/armazenagem/retirar")]
        [HttpPost]
        public async Task<IHttpActionResult> RetirarVolumeEndereco(RetirarVolumeEnderecoModelRequisicao requisicao)
        {
            try
            {
                await _armazenagemService.RetirarVolumeEndereco(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdLote ?? 0, requisicao?.IdProduto ?? 0, IdEmpresa, IdUsuario);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/ajustar/validar-endereco")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoAjustar(ValidarEnderecoAjusteModelRequisicao requisicao)
        {
            try
            {
                var validarEnderecoRequisicao = new ValidarEnderecoAjusteRequisicao
                {
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem
                };

                _armazenagemService.ValidarEnderecoAjuste(validarEnderecoRequisicao);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/ajustar/validar-lote")]
        [HttpPost]
        public IHttpActionResult ValidateLoteAjustar(ValidateLoteAjusteModelRequisicao requisicao)
        {
            try
            {
                _armazenagemService.ValidateLoteAjuste(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdLote ?? 0);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/ajustar/validar-produto")]
        [HttpPost]
        public IHttpActionResult ValidarProdutoAjustar(ValidarProdutoAjusteModelRequisicao requisicao)
        {
            try
            {
                _armazenagemService.ValidarProdutoAjuste(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdLote ?? 0, requisicao?.IdProduto ?? 0, IdEmpresa);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/ajustar/validar-quantidade")]
        [HttpPost]
        public IHttpActionResult ValidarQuantidadeAjustar(ValidarQuantidadeInstalacaoModelRequisicao requisicao)
        {
            try
            {
                var validarQuantidadeRequisicao = new ValidarQuantidadeAjusteRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    Quantidade = requisicao.Quantidade,
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    IdEmpresa = IdEmpresa
                };

                _armazenagemService.ValidarQuantidadeAjuste(validarQuantidadeRequisicao);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/ajustar")]
        [HttpPost]
        public async Task<IHttpActionResult> AjustarVolumeLote(AjustarVolumeLoteModelRequisicao requisicao)
        {
            try
            {
                var instalarVolumeLoteRequisicao = new AjustarVolumeLoteRequisicao
                {
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    Quantidade = requisicao.Quantidade,
                    IdEmpresa = IdEmpresa,
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    IdUsuarioAjuste = IdUsuario
                };

                await _armazenagemService.AjustarVolumeLote(instalarVolumeLoteRequisicao);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/abastecer/validar-endereco")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoAbastecer(ValidarEnderecoAbastecerModelRequisicao requisicao)
        {
            try
            {
                _armazenagemService.ValidarEnderecoAbastecer((requisicao?.IdEnderecoArmazenagem).GetValueOrDefault());
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/detalhes-picking/{idEnderecoArmazenagem}")]
        [HttpGet]
        public IHttpActionResult ConsultaDetalhesEnderecoPicking(long idEnderecoArmazenagem)
        {
            try
            {
                var detalhesEnderecoPicking = _armazenagemService.ConsultaDetalhesEnderecoPicking(idEnderecoArmazenagem, IdEmpresa);

                var response = new DetalhesEnderecoPickingResposta
                {
                    IdLoteProdutoEndereco = detalhesEnderecoPicking.IdLoteProdutoEndereco,
                    IdEmpresa = detalhesEnderecoPicking.IdEmpresa,
                    IdLote = detalhesEnderecoPicking.IdLote,
                    IdProduto = detalhesEnderecoPicking.IdProduto,
                    ReferenciaProduto = detalhesEnderecoPicking.Produto.Referencia,
                    IdEnderecoArmazenagem = detalhesEnderecoPicking.IdEnderecoArmazenagem,
                    Quantidade = detalhesEnderecoPicking.Quantidade,
                    CodigoUsuarioInstalacao = detalhesEnderecoPicking.AspNetUsers.UserName,
                    DataHoraInstalacao = detalhesEnderecoPicking.DataHoraInstalacao,
                    PesoTotal = detalhesEnderecoPicking.PesoTotal
                };

                return ApiOk(response);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [Route("api/v1/armazenagem/abastecer/validar-lote")]
        [HttpPost]
        public IHttpActionResult ValidateLoteAbastecer(ValidateLoteAbastecerModelRequisicao requisicao)
        {
            try
            {
                _armazenagemService.ValidarLoteAbastecer(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdLote ?? 0, requisicao?.IdProduto ?? 0);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/abastecer/validar-quantidade")]
        [HttpPost]
        public IHttpActionResult ValidarQuantidadeAbastecer(ValidarQuantidadeeAbastecerModelRequisicao requisicao)
        {
            try
            {
                _armazenagemService.ValidarQuantidadeAbastecer(requisicao?.IdEnderecoArmazenagem ?? 0,
                                                                requisicao?.IdLote ?? 0,
                                                                requisicao?.IdProduto ?? 0,
                                                                requisicao?.Quantidade ?? 0,
                                                                IdEmpresa);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/abastecer")]
        [HttpPost]
        public async Task<IHttpActionResult> AbastecerPicking(AbastecerPickingModelRequisicao requisicao)
        {
            try
            {
                await _armazenagemService.AbastecerPicking(requisicao?.IdEnderecoArmazenagem ?? 0,
                                                           requisicao?.IdLote ?? 0,
                                                           requisicao?.IdProduto ?? 0,
                                                           requisicao?.Quantidade ?? 0,
                                                           IdEmpresa,
                                                           IdUsuario);


            }
            catch (BusinessException exception)
            {
                return ApiBadRequest(exception.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/lote/produto/{idProduto}")]
        [HttpGet]
        public IHttpActionResult PesquisaLotesInstaladosProduto(long idProduto)
        {
            try
            {
                var loteInstaladoProdutoResposta = _armazenagemService.PesquisaLotesInstaladosProduto(idProduto, IdEmpresa);

                return ApiOk(loteInstaladoProdutoResposta);
            }
            catch (BusinessException exception)
            {
                return ApiBadRequest(exception.Message);
            }
        }

        [Route("api/v1/armazenagem/conferir/validar-endereco/{idEnderecoArmazenagem}")]
        [HttpPost]
        public IHttpActionResult ValidarEnderecoConferir(long idEnderecoArmazenagem)
        {
            try
            {
                _armazenagemService.ValidarEnderecoConferir(idEnderecoArmazenagem);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/conferir/validar-lote")]
        [HttpPost]
        public IHttpActionResult ValidarLoteConferir(ValidarLoteConferirModelRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _armazenagemService.ValidarLoteConferir(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdProduto ?? 0, requisicao?.IdLote ?? 0, IdEmpresa);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/conferir/validar-produto")]
        [HttpPost]
        public IHttpActionResult ValidarProdutoConferir(ValidarProdutoConferirModelRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                _armazenagemService.ValidarProdutoConferir(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdProduto ?? 0);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/conferir")]
        [HttpPost]
        public async Task<IHttpActionResult> FinalizarConferencia(FinalizarConferenciaModelRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _armazenagemService.FinalizarConferencia(requisicao?.IdEnderecoArmazenagem ?? 0, requisicao?.IdProduto ?? 0, requisicao?.IdLote ?? 0, requisicao?.Quantidade ?? 0, IdEmpresa, IdUsuario, requisicao.ConferenciaManual);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }

            return ApiOk();
        }

        [HttpGet]
        [Route("api/v1/armazenagem/pesquisar/")]
        public IHttpActionResult PesquisarPorCorredor(int corredor)
        {
            if (corredor <= 0)
            {
                return ApiBadRequest("O corredor deve ser informado.");
            }

            var pontos = _armazenagemService.PesquisarPorCorredor(corredor, IdEmpresa);

            var resposta = new NiveisPontosArmazenagemPorCorredorResposta { Lista = new List<NivelPontoArmazenagemPorCorredorResposta>() };

            foreach (var enderecoArmazenagem in pontos)
            {
                var itemResposta = new NivelPontoArmazenagemPorCorredorResposta
                {
                    IdNivelArmazenagem = enderecoArmazenagem.IdNivelArmazenagem,
                    IdPontoArmazenagem = enderecoArmazenagem.IdPontoArmazenagem,
                    PontoArmazenagemDescricao = enderecoArmazenagem.Descricao,
                    NivelArmazenagemDescricao = enderecoArmazenagem.NivelArmazenagem.Descricao,
                };

                resposta.Lista.Add(itemResposta);
            }

            resposta.Lista = resposta.Lista.OrderBy(o => o.NivelArmazenagemDescricao).ThenBy(t => t.PontoArmazenagemDescricao).ToList();

            return ApiOk(resposta);
        }

        [HttpGet]
        [Route("api/v1/armazenagem/pesquisar/{idPontoArmazenagem}/{corredor}")]
        public IHttpActionResult PesquisarNivelPontoCorredor(long idPontoArmazenagem, int corredor)
        {
            if (idPontoArmazenagem <= 0)
            {
                return ApiBadRequest("O idPontoArmazenagem deve ser informado.");
            }

            if (corredor <= 0)
            {
                return ApiBadRequest("O corredor deve ser informado.");
            }

            var enderecosArmazenagem = _armazenagemService.PesquisarNivelPontoCorredor(idPontoArmazenagem, corredor, IdEmpresa);

            var resposta = new EnderecosProdutosResposta()
            {
                Lista = enderecosArmazenagem
            };

            return ApiOk(resposta);
        }

        [HttpGet]
        [Route("api/v1/armazenagem/consultar-quantidades/{idAtividadeEstoque}")]
        public IHttpActionResult ConsultarQuantidadesPorAtividadeEstoque(long idAtividadeEstoque)
        {
            try
            {
                var enderecoProdutoQuantidade = _armazenagemService.ConsultarQuantidadesPorAtividadeEstoque(idAtividadeEstoque);

                var resposta = new EnderecoProdutoQuantidadesResposta()
                {
                    QuantidadeAtual = enderecoProdutoQuantidade.QuantidadeAtual,
                    EstoqueMinimo = enderecoProdutoQuantidade.EstoqueMinimo,
                    EstoqueMaximo = enderecoProdutoQuantidade.EstoqueMaximo
                };

                return ApiOk(resposta);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/v1/armazenagem/conferir-alas/confirma-quantidade")]
        public IHttpActionResult ConfirmaQuantidadeConferenciaAlas(ConfirmaQuantidadeConferenciaAlasRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }
            try
            {

                _armazenagemService.ConfirmaQuantidadeConferenciaAlas(requisicao?.Quantidade ?? 0, requisicao?.IdProduto ?? 0,
                    requisicao?.IdLote, requisicao?.IdEnderecoArmazenagem ?? 0, IdEmpresa, IdUsuario);

                return ApiOk();
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }
    }
}