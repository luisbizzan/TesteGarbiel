using FWLog.AspNet.Identity;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.EnumsAndConsts;
using FWLog.Web.Api.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace FWLog.Web.Api
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<WebApiUserManager>(CreateUserManagerOwin);
            app.CreatePerOwinContext<ApplicationRoleManager>(CreateRoleManagerOwin);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/v1/token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),                
                AllowInsecureHttp = true
            };

            app.UseOAuthBearerTokens(OAuthOptions);
        }

        static WebApiUserManager CreateUserManagerOwin(IdentityFactoryOptions<WebApiUserManager> options,
            IOwinContext context)
        {
            return WebApiUserManager.Create(options, context, App.Id);
        }

        static ApplicationRoleManager CreateRoleManagerOwin(IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return ApplicationRoleManager.Create(options, context, App.Id);
        }

        static ApplicationDbContext CreateManagers(out WebApiUserManager userManager, out ApplicationRoleManager roleManager)
        {
            var context = ApplicationDbContext.Create();
            var userStore = new ApplicationUserStore(context, App.Id);
            var roleStore = new ApplicationRoleStore(context, App.Id);
            userManager = new WebApiUserManager(userStore);
            roleManager = new ApplicationRoleManager(roleStore);

            return context;
        }
    }
}