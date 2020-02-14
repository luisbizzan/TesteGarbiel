using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity
{
    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        private ApplicationRoleStore _appRoleStore;

        public ApplicationRoleManager(ApplicationRoleStore roleStore)
            : base(roleStore)
        {
            _appRoleStore = roleStore;
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context, int idApplication)
        {
            return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>(), idApplication));
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, IEnumerable<string> permissions, long idEmpresa, string idUsuario)
        {
            role.Id = Guid.NewGuid().ToString();
            await _appRoleStore.CreateAsync(role, permissions, idEmpresa,idUsuario);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, IEnumerable<string> permissions)
        {
            await _appRoleStore.UpdateAsync(role, permissions);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> CreatePermissionAsync(ApplicationPermission permission)
        {
            permission.Id = Guid.NewGuid().ToString();
            await _appRoleStore.CreatePermissionAsync(permission);
            return IdentityResult.Success;
        }

        public IdentityResult CreatePermission(ApplicationPermission permission)
        {
            permission.Id = Guid.NewGuid().ToString();
            _appRoleStore.CreatePermissionAsync(permission).Wait();
            return IdentityResult.Success;
        }

        public async Task<ApplicationPermission> FindPermissionByNameAsync(string permissionName)
        {
            return await _appRoleStore.FindPermissionByNameAsync(permissionName);
        }

        public ApplicationPermission FindPermissionByName(string permissionName)
        {
            return _appRoleStore.FindPermissionByNameAsync(permissionName).Result;
        }

        public async Task<IdentityResult> AddToPermissionsAsync(ApplicationRole role, IEnumerable<string> permissions)
        {
            await _appRoleStore.AddToPermissionsAsync(role, permissions);
            return IdentityResult.Success;
        }

        public IdentityResult AddToPermissions(ApplicationRole role, IEnumerable<string> permissions)
        {
            _appRoleStore.AddToPermissionsAsync(role, permissions).Wait();
            return IdentityResult.Success;
        }

        public Task<IList<string>> GetPermissionsAsync(ApplicationRole role)
        {
            return _appRoleStore.GetPermissionsAsync(role);
        }

        public IList<string> GetPermissions(ApplicationRole role)
        {
            return _appRoleStore.GetPermissionsAsync(role).Result;
        }
    }
}
