using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity
{
    public abstract class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private ApplicationUserStore _appUserStore;

        public ApplicationUserManager(ApplicationUserStore store)
            : base(store)
        {
            _appUserStore = store;
        }

        public Task<IList<string>> GetPermissionsAsync(string userId)
        {
            return _appUserStore.GetPermissionsAsync(new ApplicationUser { Id = userId });
        }

        public Task<IList<string>> GetPermissionsByIdEmpresaAsync(string userId, long idEmpresa)
        {
            return _appUserStore.GetPermissionsByIdEmpresaAsync(new ApplicationUser { Id = userId }, idEmpresa);
        }

        public IList<string> GetPermissions(string userId)
        {
            return _appUserStore.GetPermissionsAsync(new ApplicationUser { Id = userId }).Result;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> rolesIgnorar, long idEmpresa)
        {
            await _appUserStore.UpdateAsync(user, roles, rolesIgnorar, idEmpresa);
            return IdentityResult.Success;
        }

        public IdentityResult Update(ApplicationUser user, IEnumerable<string> roles, int idEmpresa)
        {
            _appUserStore.UpdateAsync(user, roles, null, idEmpresa).Wait();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddToPermissionsAsync(ApplicationUser user, IEnumerable<string> permissions)
        {
            await _appUserStore.AddToPermissionsAsync(user, permissions);
            return IdentityResult.Success;
        }

        public IdentityResult AddToPermissions(ApplicationUser user, IEnumerable<string> permissions)
        {
            _appUserStore.AddToPermissionsAsync(user, permissions).Wait();
            return IdentityResult.Success;
        }

        public IdentityResult AddToRolesByEmpresa(ApplicationUser user, IEnumerable<string> roles, long idEmpresa)
        {
            _appUserStore.AddToRolesByEmpresa(user, roles, idEmpresa).Wait();
            return IdentityResult.Success;
        }

        public Task<IList<string>> GetUserRolesByIdEmpresa(string userId, long idEmpresa)
        {
            return _appUserStore.GetUserRolesByIdEmpresa(new ApplicationUser { Id = userId }, idEmpresa);
        }
    }
}
