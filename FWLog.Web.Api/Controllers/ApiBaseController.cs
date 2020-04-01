using FWLog.AspNet.Identity;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace FWLog.Web.Api.Controllers
{
    [ApplicationAuthorize]
    public abstract class ApiBaseController : ApiController
    {
        private WebApiUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationSignInManager _signInManager;

        protected WebApiUserManager UserManager
        {
            get => _userManager ?? (_userManager = Request.GetOwinContext().GetUserManager<WebApiUserManager>());
        }

        protected ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? (_roleManager = Request.GetOwinContext().GetUserManager<ApplicationRoleManager>());
        }

        protected ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? (_signInManager = Request.GetOwinContext().GetUserManager<ApplicationSignInManager>());
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get => Request.GetOwinContext().Authentication;
        }

        protected ApiBaseController()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }

        protected IHttpActionResult ApiOk()
        {
            return Ok();
        }

        protected IHttpActionResult ApiOk<T>(T content)
        {
            return Ok(content);
        }

        protected IHttpActionResult ApiBadRequest(string message)
        {
            LogHelper.Warn(message);
            var apiErrorResponse = ApiErrorBuilder.BuildResponse(message);
            return Content(HttpStatusCode.BadRequest, apiErrorResponse);
        }

        protected IHttpActionResult ApiBadRequest(string message, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                LogHelper.Warn(message);
            }
            else
            {
                var logWarnMessage = string.Format("{0}: UserName {1}", message, userName);
                LogHelper.Warn(logWarnMessage);
            }

            var apiErrorResponse = ApiErrorBuilder.BuildResponse(message);
            return Content(HttpStatusCode.BadRequest, apiErrorResponse);
        }

        protected IHttpActionResult ApiBadRequest(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new ArgumentNullException("modelState");
            }

            var apiErrorResponse = ApiErrorBuilder.BuildResponse(modelState);
            LogHelper.Warn(apiErrorResponse.ToString());
            return Content(HttpStatusCode.BadRequest, apiErrorResponse);
        }

        protected IHttpActionResult ApiUnauthorized(string message)
        {
            LogHelper.Warn(message);
            return ApiUnauthorized(message, string.Empty);
        }

        protected IHttpActionResult ApiUnauthorized(string message, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                LogHelper.Warn(message);
            }
            else
            {
                var logWarnMessage = string.Format("{0}: UserName {1}", message, userName);
                LogHelper.Warn(logWarnMessage);
            }

            var apiErrorResponse = ApiErrorBuilder.BuildResponse(message);
            return Content(HttpStatusCode.Unauthorized, apiErrorResponse);
        }

        protected IHttpActionResult ApiForbidden(string message)
        {
            LogHelper.Warn(message);
            return ApiForbidden(message, string.Empty);
        }

        protected IHttpActionResult ApiForbidden(string message, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                LogHelper.Warn(message);
            }
            else
            {
                var logWarnMessage = string.Format("{0}: UserName {1}", message, userName);
                LogHelper.Warn(logWarnMessage);
            }

            var apiErrorResponse = ApiErrorBuilder.BuildResponse(message);
            return Content(HttpStatusCode.Forbidden, apiErrorResponse);
        }

        protected IHttpActionResult ApiNotFound(string message)
        {
            LogHelper.Warn(message);
            var apiErrorResponse = ApiErrorBuilder.BuildResponse(message);
            return Content(HttpStatusCode.NotFound, apiErrorResponse);
        }

        protected IHttpActionResult ApiInternalServerErrror(string mensagem, Exception ex)
        {
            LogHelper.Error(ex);
            var apiErrorResponse = ApiErrorBuilder.BuildResponse(mensagem);
            return Content(HttpStatusCode.InternalServerError, apiErrorResponse);
        }

        protected string IdUsuario
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        protected long IdEmpresa
        {
            get
            {
                return RetornaIdEmpresaHeader();
            }
        }

        protected int IdAplicacao
        {
            get
            {
                return RetornaIdAplicacaoHeader();
            }
        }

        private long RetornaIdEmpresaHeader()
        {
            Request.Headers.TryGetValues("X-FWLOG-API-IdEmpresa", out IEnumerable<string> idEmpresaHeader);

            if (idEmpresaHeader != null && idEmpresaHeader.Any())
            {
                long.TryParse(idEmpresaHeader.FirstOrDefault(), out long idEmpresa);
                return idEmpresa;
            }

            return 0;
        }

        private int RetornaIdAplicacaoHeader()
        {
            Request.Headers.TryGetValues("X-FWLOG-API-IdAplicacao", out IEnumerable<string> idAplicacaoHeader);

            if (idAplicacaoHeader != null && idAplicacaoHeader.Any())
            {
                int.TryParse(idAplicacaoHeader.FirstOrDefault(), out int idAplicacao);
                return idAplicacao;
            }

            return 0;
        }
    }
}