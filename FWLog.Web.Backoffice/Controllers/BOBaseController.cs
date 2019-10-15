using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.App_Start;
using FWLog.Web.Backoffice.Helpers;
using DartDigital.Library.Web.Globalization;
using DartDigital.Library.Web.IO;
using DartDigital.Library.Web.Security.Backoffice;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace FWLog.Web.Backoffice.Controllers
{
    [ApplicationAuthorize]
    public abstract class BOBaseController : Controller
    {
        private BackofficeUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationSignInManager _signInManager;

        protected Notify Notify { get; private set; }
        protected FileHelper FileHelper { get; private set; }
        protected WebAppCultureManager CultureManager { get; private set; }

        protected IAuthenticationManager AuthenticationManager
        {
            get => HttpContext.GetOwinContext().Authentication;
        }

        protected BackofficeUserManager UserManager
        {
            get => _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<BackofficeUserManager>());
        }

        protected ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? (_roleManager = HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>());
        }

        protected ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? (_signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>());
        }

        public BOBaseController()
        {
            Notify = new Notify(TempData);
            FileHelper = FileHelper.Current;
            CultureManager = WebAppCultureManager.Current;
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            WebAppCultureManager.Current.HandleThreadCultureForRequest(Thread.CurrentThread, HttpContext);
            return base.BeginExecuteCore(callback, state);
        }
    }

}
