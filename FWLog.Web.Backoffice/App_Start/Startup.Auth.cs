using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.App_Start;
using FWLog.Web.Backoffice.EnumsAndConsts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

namespace Identity
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<BackofficeUserManager>(CreateUserManagerOwin);
            app.CreatePerOwinContext<ApplicationRoleManager>(CreateRoleManagerOwin);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<BackofficeUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromDays(10),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }

        private static BackofficeUserManager CreateUserManagerOwin(IdentityFactoryOptions<BackofficeUserManager> options, IOwinContext context)
        {
            return BackofficeUserManager.Create(options, context, App.Id);
        }

        private static ApplicationRoleManager CreateRoleManagerOwin(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return ApplicationRoleManager.Create(options, context, App.Id);
        }

        private static ApplicationDbContext CreateManagers(out BackofficeUserManager userManager, out ApplicationRoleManager roleManager)
        {
            var context = ApplicationDbContext.Create();
            var userStore = new ApplicationUserStore(context, App.Id);
            var roleStore = new ApplicationRoleStore(context, App.Id);
            userManager = new BackofficeUserManager(userStore);
            roleManager = new ApplicationRoleManager(roleStore);

            return context;
        }
    }
}