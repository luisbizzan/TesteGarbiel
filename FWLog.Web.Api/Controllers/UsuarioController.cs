﻿using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Usuario;
using FWLog.Services.Services;
using FWLog.Web.Api.Models.Usuario;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    public class UsuarioController : ApiBaseController
    {
        private readonly AccountService _accountService;
        private readonly UnitOfWork _unitOfWork;

        public UsuarioController(UnitOfWork unitOfWork, AccountService accountService)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/v1/usuario/validar")]
        public async Task<IHttpActionResult> ValidarUsuario(ValidarUsuarioRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            ApplicationUser usuarioAplicacao = await UserManager.FindByNameAsync(requisicao.Codigo);

            if (usuarioAplicacao == null)
            {
                return ApiNotFound("Usuário não cadastrado.");
            }

            PerfilUsuario usuarioPerfil = _unitOfWork.PerfilUsuarioRepository.GetByUserId(usuarioAplicacao.Id);

            if (usuarioPerfil == null)
            {
                return ApiNotFound("Usuário não cadastrado.");
            }

            if (usuarioPerfil.Ativo == false)
            {
                return ApiForbidden("Usuário inativo.");
            }

            if (usuarioPerfil.UsuarioEmpresas.Count == 0)
            {
                return ApiForbidden("Usuário sem empresa.");
            }

            return ApiOk();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/usuario/login")]
        public async Task<IHttpActionResult> Login(LoginRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            var usuarioAplicacao = await UserManager.FindByNameAsync(requisicao.Codigo);

            if (usuarioAplicacao == null)
            {
                return ApiNotFound("Usuário não cadastrado.");
            }

            PerfilUsuario usuarioPerfil = _unitOfWork.PerfilUsuarioRepository.GetByUserId(usuarioAplicacao.Id);

            if (usuarioPerfil == null)
            {
                return ApiNotFound("Usuário não cadastrado.");
            }

            if (usuarioPerfil.Ativo == false)
            {
                return ApiForbidden("Usuário inativo.", requisicao.Codigo);
            }

            if (usuarioPerfil.UsuarioEmpresas.Count == 0)
            {
                return ApiForbidden("Usuário sem empresa.", requisicao.Codigo);
            }

            SignInStatus resultadoLogin = await SignInManager.PasswordSignInAsync(requisicao.Codigo, requisicao.Senha, false, shouldLockout: true);

            if (resultadoLogin == SignInStatus.Failure)
            {
                return ApiBadRequest("Senha inválida.", requisicao.Codigo);
            }

            if (resultadoLogin == SignInStatus.LockedOut)
            {
                return ApiForbidden("Usuário bloqueado.", requisicao.Codigo);
            }

            IList<string> usuarioPermissoes = await UserManager.GetPermissionsAsync(usuarioAplicacao.Id);

            if (usuarioPermissoes == null || !usuarioPermissoes.Any(w =>
                w.Equals(Permissions.ColetorAcesso.AcessarRFArmazenagem, StringComparison.OrdinalIgnoreCase) ||
                w.Equals(Permissions.ColetorAcesso.AcessarRFSeparacao, StringComparison.OrdinalIgnoreCase) ||
                w.Equals(Permissions.ColetorAcesso.AcessarRFExpedicao, StringComparison.OrdinalIgnoreCase)))
            {
                return ApiForbidden("Usuário sem permissão.", requisicao.Codigo);
            }

            GerarTokenAcessoColetorResponse tokenResposta = await _accountService.GerarTokenAcessoColetor(requisicao.Codigo, requisicao.Senha, usuarioAplicacao.Id);

            usuarioAplicacao.IdApplicationSession = tokenResposta.ApplicationSession.IdApplicationSession;
            UserManager.Update(usuarioAplicacao);

            var empresasUsuario = Mapper.Map<List<EmpresaModelResponse>>(tokenResposta.EmpresasUsuario);

            var response = new LoginResposta
            {
                AccessToken = tokenResposta.Token.AccessToken,
                TokenType = tokenResposta.Token.TokenType,
                Empresas = empresasUsuario.OrderBy(o => o.Sigla).ToList()
            };

            return ApiOk(response);
        }

        [HttpPost]
        [Route("api/v1/usuario/logout")]
        public async Task<IHttpActionResult> Logout()
        {
            if (string.IsNullOrWhiteSpace(IdUsuario))
            {
                AuthenticationManager.SignOut();
                return ApiOk();
            }

            ApplicationUser usuarioAplicacao = UserManager.FindById(IdUsuario);

            if (usuarioAplicacao != null)
            {
                if (usuarioAplicacao.IdApplicationSession.HasValue)
                {
                    ApplicationSession usuarioSessao = _unitOfWork.ApplicationSessionRepository.GetById(usuarioAplicacao.IdApplicationSession.Value);

                    usuarioSessao.DataLogout = DateTime.Now;
                    usuarioSessao.DataUltimaAcao = DateTime.Now;

                    _unitOfWork.ApplicationSessionRepository.Update(usuarioSessao);
                    await _unitOfWork.SaveChangesAsync();

                    usuarioAplicacao.IdApplicationSession = null;
                    UserManager.Update(usuarioAplicacao);
                }
            }

            AuthenticationManager.SignOut();
            return ApiOk();
        }

        [HttpGet]
        [Route("api/v1/usuario/permissao")]
        public async Task<IHttpActionResult> BuscarPermissoes()
        {
            var resposta = new BuscarPermissoesResposta
            {
                IdUsuario = IdUsuario,
                IdEmpresa = IdEmpresa,
                Permissoes = await UserManager.GetPermissionsByIdEmpresaAsync(IdUsuario, IdEmpresa)
            };

            return ApiOk(resposta);
        }

        [HttpPost]
        [Route("api/v1/usuario/validar-permissao")]
        public async Task<IHttpActionResult> ValidaPermissao(ValidarPermissaoRequisicao requisicao)
        {
            if (requisicao == null)
            {
                return ApiBadRequest("Infome dados para requisição.");
            }

            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            var usuarioAplicacao = await UserManager.FindByNameAsync(requisicao.Codigo);

            if (usuarioAplicacao == null)
            {
                return ApiNotFound("Usuário não cadastrado.");
            }

            var retornoLogin = SignInManager.UserManager.CheckPassword(usuarioAplicacao, requisicao.Senha);

            if (!retornoLogin)
            {
                return ApiNotFound("Usuário ou senha inválidos.");
            }

            var usuarioTemPermissao = await UserManager.ValidatePermissionByIdEmpresaAsync(usuarioAplicacao.Id, IdEmpresa, requisicao.Permissao);

            if (!usuarioTemPermissao)
            {
                return ApiForbidden("Usuário sem permissão.", requisicao.Codigo);
            }

            return ApiOk(new ValidarPermissaoResposta());
        }
    }
}