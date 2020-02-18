using AutoMapper;
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

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/usuario/login")]
        public async Task<IHttpActionResult> Login(LoginModelRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            ApplicationUser applicationUser = await UserManager.FindByNameAsync(loginRequest.Usuario);

            if(applicationUser == null)
            {
                return ApiNotFound("O usuário informado não foi encontrado.");
            }

            PerfilUsuario perfilUsuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(applicationUser.Id);

            if (perfilUsuario == null)
            {
                return ApiNotFound("O usuário informado não foi encontrado.");
            }

            if (perfilUsuario.Ativo == false)
            {
                return ApiForbidden("O usuário não está ativo no sistema.", loginRequest.Usuario);
            }

            if(perfilUsuario.UsuarioEmpresas.Count == 0)
            {
                return ApiForbidden("O usuário não está vinculado a uma empresa.", loginRequest.Usuario);
            }

            SignInStatus signInResult = await SignInManager.PasswordSignInAsync(loginRequest.Usuario, loginRequest.Senha, false, shouldLockout: true);

            if (signInResult == SignInStatus.Failure)
            {
                return ApiBadRequest("Usuário ou senha inválidos.", loginRequest.Usuario);
            }

            if (signInResult == SignInStatus.LockedOut)
            {
                return ApiForbidden("O usuário está bloqueado para acessar a aplicação.", loginRequest.Usuario);
            }

            IList<string> userPermissions = await UserManager.GetPermissionsAsync(applicationUser.Id);

            if (userPermissions == null || !userPermissions.Any(w => w.Equals("RFAcessoLogin", StringComparison.OrdinalIgnoreCase)))
            {
                return ApiForbidden("O usuário não tem permissão para acessar a aplicação.", loginRequest.Usuario);
            }

            GerarTokenAcessoColetorResponse tokenRespnse = await _accountService.GerarTokenAcessoColetor(loginRequest.Usuario, loginRequest.Senha, applicationUser.Id);
            
            applicationUser.IdApplicationSession = tokenRespnse.ApplicationSession.IdApplicationSession;
            UserManager.Update(applicationUser);

            var empresasUsuario = Mapper.Map<List<EmpresaModelResponse>>(tokenRespnse.EmpresasUsuario);

            var response = new LoginModelResponse
            {
                AccessToken = tokenRespnse.Token.AccessToken,
                TokenType = tokenRespnse.Token.TokenType,
                Empresas = empresasUsuario.OrderBy(o => o.Sigla).ToList()
            };

            return ApiOk(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/v1/usuario/logout")]
        public async Task<IHttpActionResult> Logout()
        {
            ApplicationUser applicationUser = await UserManager.FindByNameAsync(User.Identity.Name);

            if (applicationUser != null)
            {
                if (applicationUser.IdApplicationSession.HasValue)
                {
                    ApplicationSession applicationSession = _unitOfWork.ApplicationSessionRepository.GetById(applicationUser.IdApplicationSession.Value);

                    applicationSession.DataLogout = DateTime.Now;
                    applicationSession.DataUltimaAcao = DateTime.Now;

                    _unitOfWork.ApplicationSessionRepository.Update(applicationSession);
                    _unitOfWork.SaveChanges();

                    applicationUser.IdApplicationSession = null;
                    UserManager.Update(applicationUser);
                }
            }

            AuthenticationManager.SignOut();
            return ApiOk();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/v1/usuario/validar")]
        public async Task<IHttpActionResult> ValidarUsuario(ValidarUsuarioRequisicao requisicao)
        {
            ApplicationUser usuarioAplicacao = await UserManager.FindByNameAsync(requisicao.Codigo);

            if(usuarioAplicacao == null)
            {
                return ApiNotFound("Usuário não cadastrado.");
            }

            PerfilUsuario usuarioPerfil = _unitOfWork.PerfilUsuarioRepository.GetByUserId(usuarioAplicacao.Id);

            if(usuarioPerfil == null)
            {
                return ApiNotFound("Usuário não cadastrado.");
            }

            if(usuarioPerfil.Ativo == false)
            {
                return ApiForbidden("Usuário inativo.");
            }

            if(usuarioPerfil.UsuarioEmpresas.Count == 0)
            {
                return ApiForbidden("Usuário sem empresa.");
            }

            return ApiOk();
        }
    }
}