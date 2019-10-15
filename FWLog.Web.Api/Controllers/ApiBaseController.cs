using FWLog.AspNet.Identity;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.Helpers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    [ApplicationAuthorize]
    public class ApiBaseController : ApiController
    {
        private WebApiUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public WebApiUserManager UserManager
        {
            get => _userManager ?? (_userManager = Request.GetOwinContext().GetUserManager<WebApiUserManager>());
        }

        protected ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? (_roleManager = Request.GetOwinContext().GetUserManager<ApplicationRoleManager>());
        }


        protected IAuthenticationManager AuthenticationManager
        {
            get => Request.GetOwinContext().Authentication;
        }

        public ApiBaseController()
        {

        }
    }
}