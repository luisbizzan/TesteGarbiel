using FWLog.AspNet.Identity;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.Helpers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using static FWLog.Web.Api.App_Start.WebApiUserManager;

namespace FWLog.Web.Api.Controllers
{
    [ApplicationAuthorize]
    public class ApiBaseController : ApiController
    {
        private WebApiUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationSignInManager _signInManager;

        public WebApiUserManager UserManager
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

        public ApiBaseController()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }

        public IHttpActionResult ApiOk()
        {
            return Ok();
        }

        public IHttpActionResult ApiOk<T>(T content)
        {
            return Ok(content);
        }

        public IHttpActionResult ApiBadRequest(string message)
        {
            LogHelper.Warn(message);
            var apiErrorResponse = ApiErrorBuilder.BuildResponse(message);
            return Content(HttpStatusCode.BadRequest, apiErrorResponse);
        }

        public IHttpActionResult ApiBadRequest(string message, string userName)
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

        public IHttpActionResult ApiBadRequest(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new ArgumentNullException("modelState");
            }

            var apiErrorResponse = ApiErrorBuilder.BuildResponse(modelState);
            LogHelper.Warn(apiErrorResponse.ToString());
            return Content(HttpStatusCode.BadRequest, apiErrorResponse);
        }

        public IHttpActionResult ApiUnauthorized(string message)
        {
            LogHelper.Warn(message);
            return ApiUnauthorized(message, string.Empty);
        }

        public IHttpActionResult ApiUnauthorized(string message, string userName)
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

        public IHttpActionResult ApiForbidden(string message)
        {
            LogHelper.Warn(message);
            return ApiForbidden(message, string.Empty);
        }

        public IHttpActionResult ApiForbidden(string message, string userName)
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

        public IHttpActionResult ApiNotFound(string message)
        {
            LogHelper.Warn(message);
            var apiErrorResponse = ApiErrorBuilder.BuildResponse(message);
            return Content(HttpStatusCode.NotFound, apiErrorResponse);
        }
    }
}