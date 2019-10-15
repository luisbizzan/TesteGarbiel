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
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Api
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<WebApiUserManager>(CreateUserManagerOwin);
            app.CreatePerOwinContext<ApplicationRoleManager>(CreateRoleManagerOwin);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
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