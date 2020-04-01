using FWLog.AspNet.Identity;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.GlobalResources.General;
using FWLog.Web.Api.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace FWLog.Web.Api.Helpers
{
    public class ApplicationAuthorizeAttribute : AuthorizeAttribute
    {
        public string Permissions { get; set; }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var apiError = new ApiErro
            {
                Mensagem = GeneralStrings.AuthorizationDenied
            };

            var apiErrorResponse = new ApiErroResposta
            {
                Erros = new List<ApiErro> { apiError }
            };

            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(apiErrorResponse)),
                StatusCode = HttpStatusCode.Unauthorized
            };

            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            actionContext.Response = httpResponseMessage;

            var logWarnMessage = string.Format("{0}: {1}", apiError.Mensagem, actionContext.Request.ToString());
            LogHelper.Warn(logWarnMessage);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool isAuthorized = base.IsAuthorized(actionContext);

            if (!isAuthorized)
            {
                return false;
            }

            IPrincipal user = actionContext.ControllerContext.RequestContext.Principal;

            if (user is ClaimsPrincipal == false)
            {
                return false;
            }

            string userId = user.Identity.GetUserId();
            var userManager = actionContext.Request.GetOwinContext().GetUserManager<WebApiUserManager>();
            ApplicationUser usuarioAplicacao = userManager.FindById(userId);

            if(usuarioAplicacao.IdApplicationSession.HasValue == false)
            {
                return false;
            }

            IList<string> permissions = userManager.GetPermissions(userId);
            var customUser = new ApplicationClaimsPrincipal((ClaimsPrincipal)user, permissions);
            actionContext.ControllerContext.RequestContext.Principal = customUser;
            Thread.CurrentPrincipal = customUser;

            return AuthorizeValidationHelper.UserHasPermission(actionContext, Permissions);
        }
    }
}