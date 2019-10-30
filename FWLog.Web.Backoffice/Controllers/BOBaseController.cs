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

        public void CookieSaveCompany(long companyId, string userId)
        {
            HttpCookie cookie = Request.Cookies[CompanyCookie.CookieName];

            if (companyId == 0)
            {
                if (cookie != null)
                {
                    cookie[CompanyCookie.CompanyId] = string.Empty;
                    cookie.Expires = DateTime.UtcNow.AddHours(8);
                    Response.Cookies.Add(cookie);
                }

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
                newCookie.Expires = DateTime.UtcNow.AddHours(8);
                Response.Cookies.Add(newCookie);
            }
            else
            {
                cookie.Values[CompanyCookie.CompanyId] = companyId.ToString();
                cookie.Values[CompanyCookie.ListCompanies] = Server.UrlEncode(jsonCompanies);
                cookie.Expires = DateTime.UtcNow.AddHours(8);
                Response.Cookies.Add(cookie);
            }
        }

        public int CompanyId
        {
            get
            {
                HttpCookie cookie = Request.Cookies[CompanyCookie.CookieName];

                if (cookie != null && cookie[CompanyCookie.CompanyId] != null && cookie[CompanyCookie.CompanyId] != string.Empty)
                {
                    var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
                    var userInfo = new BackOfficeUserInfo();

                    var companyId = Convert.ToInt32(cookie[CompanyCookie.CompanyId].ToString());

                    if (userInfo.UserId != null)
                    {
                        var companies = uow.CompanyRepository.GetAllByUserId(userInfo.UserId.ToString());
                        var jsonCompanies = JsonConvert.SerializeObject(companies);

                        if (companyId > 0 && companies.Where(w => w.CompanyId == companyId).ToList().Count == 0)
                        {
                            cookie[CompanyCookie.ListCompanies] = Server.UrlEncode(jsonCompanies);

                            throw new ConpanyException(string.Format("O usuário não pertence mais a empresa com o código: {0}", companyId));
                        }
                        else if (cookie.Expires < DateTime.UtcNow)
                        {
                            cookie.Values[CompanyCookie.ListCompanies] = companyId.ToString();
                            cookie.Values[CompanyCookie.ListCompanies] = Server.UrlEncode(jsonCompanies);
                            cookie.Expires = DateTime.UtcNow.AddHours(8);
                            Response.Cookies.Add(cookie);
                        }
                    }

                    return companyId;
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

                if (cookie != null && cookie[CompanyCookie.ListCompanies] != null && cookie[CompanyCookie.ListCompanies] != string.Empty)
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
