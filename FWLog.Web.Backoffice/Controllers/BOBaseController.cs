﻿using DartDigital.Library.Web.Globalization;
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

        public void CookieSalvarEmpresa(long idEmpresa, string userId, bool logoff = false)
        {
            HttpCookie cookie = Request.Cookies[EmpresaCookie.CookieName];

            if (idEmpresa == 0)
            {
                if (cookie == null)
                {
                    return;
                }

                cookie[EmpresaCookie.IdEmpresa] = string.Empty;

                if (logoff)
                {
                    cookie[EmpresaCookie.ListEmpresas] = string.Empty;
                }

                Response.Cookies.Add(cookie);

                return;
            }

            var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));

            var empresas = uow.EmpresaRepository.GetAllByUserId(userId);
            var jsonEmpresas = JsonConvert.SerializeObject(empresas);

            if (cookie == null)
            {
                var newCookie = new HttpCookie(EmpresaCookie.CookieName);
                newCookie.Values[EmpresaCookie.IdEmpresa] = idEmpresa.ToString();
                newCookie.Values[EmpresaCookie.ListEmpresas] = Server.UrlEncode(jsonEmpresas);
                newCookie.Expires = DateTime.MaxValue;
                Response.Cookies.Add(newCookie);
            }
            else
            {
                cookie.Values[EmpresaCookie.IdEmpresa] = idEmpresa.ToString();
                cookie.Values[EmpresaCookie.ListEmpresas] = Server.UrlEncode(jsonEmpresas);
                cookie.Expires = DateTime.MaxValue;
                Response.Cookies.Add(cookie);
            }
        }

        public int IdEmpresa
        {
            get
            {
                HttpCookie cookie = Request.Cookies[EmpresaCookie.CookieName];

                if (cookie != null && cookie[EmpresaCookie.IdEmpresa] != null && !string.IsNullOrEmpty(cookie[EmpresaCookie.IdEmpresa]))
                {
                    return Convert.ToInt32(cookie[EmpresaCookie.IdEmpresa].ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        public IEnumerable<EmpresaSelectedItem> Empresas
        {
            get
            {
                HttpCookie cookie = Request.Cookies[EmpresaCookie.CookieName];

                if (cookie != null && cookie[EmpresaCookie.ListEmpresas] != null && !string.IsNullOrEmpty(cookie[EmpresaCookie.ListEmpresas]))
                {
                    var jsonEmpresas = Server.UrlDecode(cookie[EmpresaCookie.ListEmpresas].ToString());
                    var empresas = JsonConvert.DeserializeObject<IEnumerable<EmpresaSelectedItem>>(jsonEmpresas);

                    return empresas;
                }
                else
                {
                    var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
                    var userInfo = new BackOfficeUserInfo();
                    return uow.EmpresaRepository.GetAllByUserId(userInfo.UserId.ToString());
                }
            }
        }
    }
}
