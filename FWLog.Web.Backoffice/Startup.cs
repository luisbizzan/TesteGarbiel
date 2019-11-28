using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.App_Start;
using FWLog.Web.Backoffice.EnumsAndConsts;
using Microsoft.AspNet.Identity;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private static void InitializePermissionManager()
        {
            BackofficeUserManager userManager;
            ApplicationRoleManager roleManager;

            using (CreateManagers(out userManager, out roleManager))
            {
                CreatePermissions(roleManager);
                CreateAdminUser(userManager);
            }
        }

        private static void CreatePermissions(ApplicationRoleManager roleManager)
        {
            Permissions permissions = new Permissions();
            PermissionManager.CreatePermissionsIfNotExists(builder: permissions, roleManager: roleManager);
        }

        private static void CreateAdminUser(BackofficeUserManager userManager)
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
