using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.App_Start;
using FWLog.Web.Backoffice.EnumsAndConsts;
using System.Web;

namespace Identity
{
    public partial class Startup
    {

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and role manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<BackofficeUserManager>(CreateUserManagerOwin);
            app.CreatePerOwinContext<ApplicationRoleManager>(CreateRoleManagerOwin);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<BackofficeUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

        }

        static BackofficeUserManager CreateUserManagerOwin(IdentityFactoryOptions<BackofficeUserManager> options,
            IOwinContext context)
        {
            return BackofficeUserManager.Create(options, context, App.Id);
        }

        static ApplicationRoleManager CreateRoleManagerOwin(IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return ApplicationRoleManager.Create(options, context, App.Id);
        }

        static ApplicationDbContext CreateManagers(out BackofficeUserManager userManager, out ApplicationRoleManager roleManager)
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