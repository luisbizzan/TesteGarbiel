﻿using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.SeparacaoPedido;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class SeparacaoPedidoController : ApiBaseController
    {
        private readonly SeparacaoPedidoService _separacaoPedidoService;

        public SeparacaoPedidoController(SeparacaoPedidoService separacaoPedidoService)
        {
            _separacaoPedidoService = separacaoPedidoService;
        }

        [Route("api/v1/separacao-pedido/consultar-pedido-venda")]
        [HttpGet]
        public IHttpActionResult ConsultaPedidoVendaEmSeparacao()
        {
            var idsPedidosProcessoDeSeparacao = _separacaoPedidoService.ConsultaPedidoVendaEmSeparacao(IdUsuario, IdEmpresa);

            var response = new ConsultaPedidoVendaEmSeparacaoResposta
            {
                PedidosVendaVolumeEmSeparacao = idsPedidosProcessoDeSeparacao
            };

            return ApiOk(response);
        }

        [Route("api/v1/separacao-pedido/consultar-pedido-venda/{referenciaPedido}")]
        [HttpGet]
        public async Task<IHttpActionResult> BuscarPedidoVenda(string referenciaPedido)
        {
            try
            {
                var permissions = await UserManager.GetPermissionsByIdEmpresaAsync(IdUsuario, IdEmpresa);

                var temPermissaoF7 = permissions.Contains(Permissions.RFSeparacao.FuncaoF7);

                var response = _separacaoPedidoService.BuscarPedidoVenda(referenciaPedido, IdEmpresa, IdUsuario, temPermissaoF7);

                return ApiOk(response);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }

        [Route("api/v1/separacao-pedido/cancelar")]
        [HttpPost]
        public async Task<IHttpActionResult> CancelarPedidoSeparacao(CancelarPedidoSeparacaoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                ApplicationUser usuarioPermissao = null;
                bool usuarioTemPermissao = false;

                if (!string.IsNullOrWhiteSpace(requisicao.UsuarioPermissao))
                {
                    usuarioPermissao = await UserManager.FindByNameAsync(requisicao.UsuarioPermissao);

                    usuarioTemPermissao = await UserManager.ValidatePermissionByIdEmpresaAsync(usuarioPermissao?.Id, IdEmpresa, Permissions.RFSeparacao.CancelarSeparacao);
                }

                await _separacaoPedidoService.CancelarPedidoSeparacao(requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.UsuarioPermissao, usuarioTemPermissao, usuarioPermissao?.Id, IdUsuario, IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/separacao-pedido/iniciar-separacao-pedido-venda")]
        [HttpPost]
        public async Task<IHttpActionResult> IniciarSeparacaoPedido(IniciarSeparacaoVolumeRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _separacaoPedidoService.IniciarSeparacaoPedidoVenda(requisicao.IdPedidoVenda, IdUsuario, IdEmpresa, requisicao.IdPedidoVendaVolume);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [AllowAnonymous]
        [Route("api/v1/separacao-pedido/dividir-pedido")]
        [HttpPost]
        public async Task<IHttpActionResult> DividirPedido(long idEmpresa)
        {
            try
            {
                await _separacaoPedidoService.DividirPedido(idEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/separacao-pedido/finalizar-volume")]
        [HttpPost]
        public async Task<IHttpActionResult> FinalizarSeparacaoVolume(FinalizarSeparacaoVolumeRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                await _separacaoPedidoService.FinalizarSeparacaoVolume(requisicao?.IdPedidoVenda ?? 0, requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.IdCaixa ?? 0, IdUsuario, IdEmpresa);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }

            return ApiOk();
        }

        [Route("api/v1/separacao-pedido/salvar-separacao-produto")]
        [HttpPost]
        public async Task<IHttpActionResult> SalvarSeparacaoProduto(SalvarSeparacaoProdutoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            try
            {
                var usuarioTemPermissaoF7 = await UserManager.ValidatePermissionByIdEmpresaAsync(IdUsuario, IdEmpresa, Permissions.RFSeparacao.FuncaoF7);

                ApplicationUser usuarioValidaPermissaoF8 = null;
                bool usuarioTemPermissaoF8 = false;

                if (!string.IsNullOrWhiteSpace(requisicao.CodigoUsuarioAutorizacaoZerarPedido))
                {
                    usuarioValidaPermissaoF8 = await UserManager.FindByNameAsync(requisicao.CodigoUsuarioAutorizacaoZerarPedido);

                    usuarioTemPermissaoF8 = await UserManager.ValidatePermissionByIdEmpresaAsync(usuarioValidaPermissaoF8?.Id, IdEmpresa, Permissions.RFSeparacao.RFFuncaoF8ZerarPedido);
                }

                var response = await _separacaoPedidoService.SalvarSeparacaoProduto(requisicao?.IdPedidoVendaVolume ?? 0, requisicao?.IdProduto ?? 0, requisicao?.IdProdutoSeparacao, IdUsuario, IdEmpresa, requisicao?.QtdAjuste, usuarioTemPermissaoF7, usuarioValidaPermissaoF8?.Id, usuarioTemPermissaoF8);

                return ApiOk(response);
            }
            catch (BusinessException businessException)
            {
                return ApiBadRequest(businessException.Message);
            }
        }

        [Route("api/v1/separacao-pedido/consultar-detalhes-pedido-venda/{referenciaOuNumeroPedido}")]
        [HttpGet]
        public IHttpActionResult ConsultarDetalhesPedidoVenda(string referenciaOuNumeroPedido)
        {
            try
            {
                var response = _separacaoPedidoService.ConsultarDetalhesPedidoVenda(referenciaOuNumeroPedido, IdEmpresa);

                return ApiOk(response);
            }
            catch (BusinessException ex)
            {
                return ApiBadRequest(ex.Message);
            }
        }
    }
}