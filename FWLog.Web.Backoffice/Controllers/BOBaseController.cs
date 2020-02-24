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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                Response.Cookies.Add(cookie);

                return;
            }

            cookie = cookie ?? new HttpCookie(EmpresaCookie.CookieName);

            cookie.Values[EmpresaCookie.IdEmpresa] = idEmpresa.ToString();
            cookie.Expires = DateTime.MaxValue;
            Response.Cookies.Add(cookie);
        }

        protected void CookieLogoff()
        {
            HttpCookie cookie = Request.Cookies[EmpresaCookie.CookieName] ?? new HttpCookie(EmpresaCookie.CookieName);

            cookie.Expires = DateTime.Now.AddDays(-1);

            Response.Cookies.Add(cookie);
        }

        public long IdEmpresa
        {
            get
            {
                HttpCookie cookie = Request.Cookies[EmpresaCookie.CookieName];

                if (cookie != null && cookie[EmpresaCookie.IdEmpresa] != null && !string.IsNullOrEmpty(cookie[EmpresaCookie.IdEmpresa]))
                {
                    return Convert.ToInt64(cookie[EmpresaCookie.IdEmpresa].ToString());
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
                var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
                var userInfo = new BackOfficeUserInfo();
                return uow.EmpresaRepository.GetAllByUserId(userInfo.UserId.ToString()).OrderBy(x => x.Nome);
            }
        }

        protected ReadOnlyCollection<long> IdEmpresasPorUsuario
        {
            get
            {
                if (idEmpresasPorUsuario == null)
                {
                    var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
                    var userInfo = new BackOfficeUserInfo();

                    idEmpresasPorUsuario = uow.EmpresaRepository.IdEmpresasPorUsuario(userInfo.UserId.ToString());
                }

                return idEmpresasPorUsuario;
            }
        }

        private ReadOnlyCollection<long> idEmpresasPorUsuario;

        protected string IdUsuario
        {
            get
            {
                var userInfo = new BackOfficeUserInfo();

                return userInfo.UserId.ToString();
            }
        }

        protected long IdPerfilImpressora
        {
            get
            {
                HttpCookie cookie = Request.Cookies[EmpresaCookie.CookieName] ?? new HttpCookie(EmpresaCookie.CookieName) { Expires = DateTime.MaxValue };

                var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));

                if (!long.TryParse(cookie.Values[EmpresaCookie.PerfilImpressora], out long idPerfilImpressora))
                {
                    idPerfilImpressora = PerfilImpressoraPadrao(cookie, uow);
                }
                else
                {
                    bool perfilDaEmpresaSelecionada = uow.PerfilImpressoraRepository.RetornarAtivas().Any(x => x.IdPerfilImpressora == idPerfilImpressora && x.IdEmpresa == IdEmpresa);

                    if (!perfilDaEmpresaSelecionada)
                    {
                        idPerfilImpressora = PerfilImpressoraPadrao(cookie, uow);
                    }
                }

                return idPerfilImpressora;
            }
            set
            {
                HttpCookie cookie = Request.Cookies[EmpresaCookie.CookieName] ?? new HttpCookie(EmpresaCookie.CookieName) { Expires = DateTime.MaxValue };

                cookie.Values[EmpresaCookie.PerfilImpressora] = value.ToString();

                Response.Cookies.Add(cookie);
            }
        }

        protected string LabelUsuario
        {
            get
            {
                var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
                var userInfo = new BackOfficeUserInfo();
                var usuario = uow.PerfilUsuarioRepository.GetByUserId(userInfo.UserId.ToString());
                return $"{usuario.Usuario.UserName} - {usuario.Nome}";
            }
        }

        private long PerfilImpressoraPadrao(HttpCookie cookie, UnitOfWork uow)
        {
            long idPerfilImpressora = uow.UsuarioEmpresaRepository.Tabela().FirstOrDefault(x => x.IdEmpresa == IdEmpresa && x.UserId == IdUsuario)?.IdPerfilImpressoraPadrao ?? 0;
            cookie.Values[EmpresaCookie.PerfilImpressora] = idPerfilImpressora.ToString();

            Response.Cookies.Add(cookie);

            return idPerfilImpressora;
        }

        public void SetViewBags()
        {
            ViewBag.Status = new SelectList(new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Ativo", Value = "true"},
                            new SelectListItem { Text = "Inativo", Value = "false"}
                        }, "Value", "Text");
        }
    }
}
