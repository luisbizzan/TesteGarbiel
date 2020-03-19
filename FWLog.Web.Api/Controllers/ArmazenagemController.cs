using FWLog.Services.Model.Armazenagem;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Armazenagem;
using System;
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
            catch
            {
                throw;
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

                _armazenagemService.ValidarEnderecoInstalacao(validarEnderecoRequisicao);
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
                    IdUsuarioInstalacao = IdUsuario
                };

                await _armazenagemService.InstalarVolumeLote(instalarVolumeLoteRequisicao);
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
            }

            return ApiOk();
        }

        [Route("api/v1/armazenagem/detalhes/{idEnderecoArmazenagem}")]
        [HttpGet]
        public IHttpActionResult ConsultaDetalhesEnderecoArmazenagem(long idEnderecoArmazenagem)
        {
            try
            {
                var detalhesEnderecoArmazenagem = _armazenagemService.ConsultaDetalhesEnderecoArmazenagem(idEnderecoArmazenagem);

                var response = new ConsultaDetalhesEnderecoArmazenagemResposta
                {
                    IdLoteProdutoEndereco = detalhesEnderecoArmazenagem.IdLoteProdutoEndereco,
                    IdEmpresa = detalhesEnderecoArmazenagem.IdEmpresa,
                    IdLote = detalhesEnderecoArmazenagem.IdLote,
                    IdProduto = detalhesEnderecoArmazenagem.IdProduto,
                    IdEnderecoArmazenagem = detalhesEnderecoArmazenagem.IdEnderecoArmazenagem,
                    Quantidade = detalhesEnderecoArmazenagem.Quantidade,
                    CodigoUsuarioInstalacao = detalhesEnderecoArmazenagem.AspNetUsers.UserName,
                    DataHoraInstalacao = detalhesEnderecoArmazenagem.DataHoraInstalacao,
                    PesoTotal = detalhesEnderecoArmazenagem.PesoTotal
                };

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
            catch
            {
                throw;
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
                                                                requisicao?.Quantidade ?? 0);
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
    }
}