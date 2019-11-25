using FWLog.Web.Api.Helpers;
using FWLog.Web.Api.Models;
using FWLog.AspNet.Identity;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.GlobalResources.General;
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
using System.Collections.Specialized;
using System;

namespace FWLog.Web.Api.Helpers
{
    public class ApplicationAuthorizeAttribute : AuthorizeAttribute
    {
        public string Permissions { get; set; }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var apiError = new ApiError
            {
                Message = GeneralStrings.AuthorizationDenied
            };

            var apiErrorResponse = new ApiErrorModelResponse
            {
                Errors = new List<ApiError> { apiError }
            };

            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(apiErrorResponse)),
                StatusCode = HttpStatusCode.Unauthorized
            };

            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            actionContext.Response = httpResponseMessage;

            var logWarnMessage = string.Format("{0}: {1}", apiError.Message, actionContext.Request.ToString());
            LogHelper.Warn(logWarnMessage);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool isAuthorized = base.IsAuthorized(actionContext);

            if (!isAuthorized)
            {
                return false;
            }

            SetPrincipal(actionContext);

            return AuthorizeValidationHelper.UserHasPermission(actionContext, Permissions);
        }

        private void SetPrincipal(HttpActionContext actionContext)
        {
            IPrincipal user = actionContext.ControllerContext.RequestContext.Principal;

            if (user is ClaimsPrincipal == false)
            {
                return;
            }

            string userId = user.Identity.GetUserId();

            NameValueCollection parameters = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);

            var userManager = actionContext.Request.GetOwinContext().GetUserManager<WebApiUserManager>();
            IList<string> permissions = userManager.GetPermissions(userId, Convert.ToInt32(parameters["IdEmpresa"]));

            var customUser = new ApplicationClaimsPrincipal((ClaimsPrincipal)user, permissions);

            actionContext.ControllerContext.RequestContext.Principal = customUser;
            Thread.CurrentPrincipal = customUser;
        }
    }
}