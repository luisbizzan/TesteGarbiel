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
using FWLog.Web.Backoffice.EnumsAndConsts;
using System.Collections.Generic;
using FWLog.Data.Models;
using System.Linq;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data;
using Newtonsoft.Json;

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

        public void CookieSaveCompany(int companyId)
        {
            var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
            var userInfo = new BackOfficeUserInfo();

            var companies = uow.CompanyRepository.GetAllByUserId(userInfo.UserId.ToString());
            var jsonCompanies = JsonConvert.SerializeObject(companies);

            if (Response.Cookies[CompanyCookie.CookieName] == null)
            {
                var cookie = new HttpCookie(CompanyCookie.CookieName);
                cookie.Values[CompanyCookie.CompanyId] = companyId.ToString();
                cookie.Values[CompanyCookie.ListCompanies] = jsonCompanies;
                cookie.Expires = DateTime.UtcNow.AddHours(8);
                Response.Cookies.Add(cookie);
            }
            else
            {
                Response.Cookies[CompanyCookie.CookieName][CompanyCookie.CompanyId] = companyId.ToString();
                Response.Cookies[CompanyCookie.CookieName][CompanyCookie.ListCompanies] = jsonCompanies;
                Response.Cookies[CompanyCookie.CookieName].Expires = DateTime.UtcNow.AddHours(8);
            }
        }

        public int? CompanyId
        {
            get
            {
                if (Request.Cookies[CompanyCookie.CookieName] != null && Request.Cookies[CompanyCookie.CookieName][CompanyCookie.CompanyId] != null)
                {
                    return Convert.ToInt32(Request.Cookies[CompanyCookie.CookieName][CompanyCookie.CompanyId].ToString());
                }
                else
                {
                    return null;
                }
                //TODO lançar erro se não encontrar empresa?
            }
        }

        public IEnumerable<CompanySelectedItem> Companies
        {
            get
            {
                if (Request.Cookies[CompanyCookie.CookieName] != null && Request.Cookies[CompanyCookie.CookieName][CompanyCookie.ListCompanies] != null)
                {
                    var jsonCompanies = Request.Cookies[CompanyCookie.CookieName][CompanyCookie.ListCompanies].ToString();
                    var companies = JsonConvert.DeserializeObject<IEnumerable<CompanySelectedItem>>(jsonCompanies);

                    return companies;
                }
                else
                {
                    var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
                    var userInfo = new BackOfficeUserInfo();
                    return uow.CompanyRepository.GetAllByUserId(userInfo.UserId.ToString());                    
                }

                //TODO lançar erro se não encontrar empresa?
            }
        }
    }
}
