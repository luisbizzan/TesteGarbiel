using FWLog.AspNet.Identity;
using FWLog.Web.Api.App_Start;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FWLog.Web.Api.Startup))]
namespace FWLog.Web.Api
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
            WebApiUserManager userManager;
            ApplicationRoleManager roleManager;

            using (CreateManagers(out userManager, out roleManager))
            {
                var managers = new Managers { UserManager = userManager, RoleManager = roleManager };
                CreatePermissions(managers);
            }
        }

        static void CreatePermissions(Managers managers)
        {
            Permissions permissions = new Permissions();
            PermissionManager.CreatePermissionsIfNotExists(builder: permissions, roleManager: managers.RoleManager);
        }

        private class Managers
        {
            public WebApiUserManager UserManager { get; set; }
            public ApplicationRoleManager RoleManager { get; set; }
        }
    }
}