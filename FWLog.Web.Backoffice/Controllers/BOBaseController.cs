using DartDigital.Library.Web.Globalization;
using DartDigital.Library.Web.IO;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Web.Backoffice.App_Start;
using FWLog.Web.Backoffice.EnumsAndConsts;
using FWLog.Web.Backoffice.Helpers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Mvc;

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

        public void CookieSaveCompany(long companyId, string userId, bool logoff = false)
        {
            HttpCookie cookie = Request.Cookies[CompanyCookie.CookieName];

            if (companyId == 0)
            {
                if (cookie == null)
                {
                    return;
                }

                cookie[CompanyCookie.CompanyId] = string.Empty;

                if (logoff)
                {
                    cookie[CompanyCookie.ListCompanies] = string.Empty;
                }

                Response.Cookies.Add(cookie);

                return;
            }

            var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));

            var companies = uow.CompanyRepository.GetAllByUserId(userId);
            var jsonCompanies = JsonConvert.SerializeObject(companies);

            if (cookie == null)
            {
                var newCookie = new HttpCookie(CompanyCookie.CookieName);
                newCookie.Values[CompanyCookie.CompanyId] = companyId.ToString();
                newCookie.Values[CompanyCookie.ListCompanies] = Server.UrlEncode(jsonCompanies);
                newCookie.Expires = DateTime.MaxValue;
                Response.Cookies.Add(newCookie);
            }
            else
            {
                cookie.Values[CompanyCookie.CompanyId] = companyId.ToString();
                cookie.Values[CompanyCookie.ListCompanies] = Server.UrlEncode(jsonCompanies);
                cookie.Expires = DateTime.MaxValue;
                Response.Cookies.Add(cookie);
            }
        }

        public int CompanyId
        {
            get
            {
                HttpCookie cookie = Request.Cookies[CompanyCookie.CookieName];

                if (cookie != null && cookie[CompanyCookie.CompanyId] != null && !string.IsNullOrEmpty(cookie[CompanyCookie.CompanyId]))
                {
                    return Convert.ToInt32(cookie[CompanyCookie.CompanyId].ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        public IEnumerable<CompanySelectedItem> Companies
        {
            get
            {
                HttpCookie cookie = Request.Cookies[CompanyCookie.CookieName];

                if (cookie != null && cookie[CompanyCookie.ListCompanies] != null && !string.IsNullOrEmpty(cookie[CompanyCookie.ListCompanies]))
                {
                    var jsonCompanies = Server.UrlDecode(cookie[CompanyCookie.ListCompanies].ToString());
                    var companies = JsonConvert.DeserializeObject<IEnumerable<CompanySelectedItem>>(jsonCompanies);

                    return companies;
                }
                else
                {
                    var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
                    var userInfo = new BackOfficeUserInfo();
                    return uow.CompanyRepository.GetAllByUserId(userInfo.UserId.ToString());
                }
            }
        }
    }
}
