using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.App_Start;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using FWLog.Web.Backoffice.EnumsAndConsts;
using System;

namespace Identity
{
    public partial class Startup
    {
        public Startup()
        {
            InitializePermissionManager();
        }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        static void InitializePermissionManager()
        {
            BackofficeUserManager userManager;
            ApplicationRoleManager roleManager;

            using (CreateManagers(out userManager, out roleManager))
            {
                CreatePermissions(roleManager);
                CreateAdminUser(userManager);
            }
        }

        static void CreatePermissions(ApplicationRoleManager roleManager)
        {
            Permissions permissions = new Permissions();
            PermissionManager.CreatePermissionsIfNotExists(builder: permissions, roleManager: roleManager);
        }

        static void CreateAdminUser(BackofficeUserManager userManager)
        {
            string email = App.UserName;

            var adminUser = userManager.FindByName(email);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = email, Email = email };
                adminUser.Id = Guid.NewGuid().ToString();
                IdentityResult result = userManager.Create(adminUser, App.UserPass);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(result.Errors.FirstOrDefault());
                }
            }

            var permissionsBuildItens = PermissionManager.Groups.SelectMany(x => x.Permissions);
            IEnumerable<string> permissions = permissionsBuildItens.Select(x => x.Name).ToList();

            userManager.AddToPermissions(adminUser, permissions);
        }
    }
}
