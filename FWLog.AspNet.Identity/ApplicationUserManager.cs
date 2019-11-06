using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task<IList<string>> GetPermissionsByCompanyIdAsync(string userId, long companyId)
        {
            return _appUserStore.GetPermissionsByCompanyIdAsync(new ApplicationUser { Id = userId }, companyId);
        }

        public IList<string> GetPermissions(string userId, int companyId)
        {
            return _appUserStore.GetPermissionsAsync(new ApplicationUser { Id = userId }).Result;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, IEnumerable<string> roles, long companyId)
        {
            await _appUserStore.UpdateAsync(user, roles, companyId);
            return IdentityResult.Success;
        }

        public IdentityResult Update(ApplicationUser user, IEnumerable<string> roles, int companyId)
        {
            _appUserStore.UpdateAsync(user, roles, companyId).Wait();
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

        public IdentityResult AddToRolesByCompany(ApplicationUser user, IEnumerable<string> roles, long companyId)
        {
            _appUserStore.AddToRolesByCompany(user, roles, companyId).Wait();
            return IdentityResult.Success;
        }

        public Task<IList<string>> GetUserRolesByCompanyId(string userId, long companyId)
        {
            return _appUserStore.GetUserRolesByCompanyId(new ApplicationUser { Id = userId }, companyId);
        }
    }
}
