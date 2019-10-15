using FWLog.AspNet.Identity;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.EnumsAndConsts;
using Microsoft.Owin;
using Owin;
using System.Linq;

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
                CreateTestRole(managers);
                CreateTestUser(managers);

                // Adicione aqui todas as roles, usuários, etc que forem necessários.
            }
        }

        static void CreatePermissions(Managers managers)
        {
            Permissions permissions = new Permissions();
            PermissionManager.CreatePermissionsIfNotExists(builder: permissions, roleManager: managers.RoleManager);
        }

        static void CreateTestRole(Managers managers)
        {
            //var role = new ApplicationRole("Test");

            //string[] permissions = new string[]
            //{
            //    Permissions.Test.Create,
            //    Permissions.Test.Edit,
            //    Permissions.Test.Delete,
            //    Permissions.Test.List
            //};

            //PermissionManager.CreateRoleIfNotExists(role, permissions, managers.RoleManager);
        }

        static void CreateTestUser(Managers managers)
        {
            string email = "teste@dartdigital.com.br";
            var user = new ApplicationUser { Email = email, UserName = email };

            PermissionManager.CreateUserIfNotExists(user, "123teste", "Test", managers.UserManager, managers.RoleManager);
        }


        private class Managers
        {
            public WebApiUserManager UserManager { get; set; }
            public ApplicationRoleManager RoleManager { get; set; }
        }
    }


}