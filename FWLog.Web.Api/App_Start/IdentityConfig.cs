using FWLog.AspNet.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Api.App_Start
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class WebApiUserManager : ApplicationUserManager
    {
        private ApplicationUserStore _appUserStore;

        public WebApiUserManager(ApplicationUserStore store)
            : base(store)
        {
            _appUserStore = store;
        }

        public static WebApiUserManager Create(IdentityFactoryOptions<WebApiUserManager> options, IOwinContext context, int idApplication)
        {
            var manager = new WebApiUserManager(new ApplicationUserStore(context.Get<ApplicationDbContext>(), idApplication));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var dataProtectionProvider = options.DataProtectionProvider;

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}