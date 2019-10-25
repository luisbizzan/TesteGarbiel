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

        public Task<IList<string>> GetPermissionsAsync(string userId, int companyId)
        {
            return _appUserStore.GetPermissionsAsync(new ApplicationUser { Id = userId }, companyId);
        }

        public IList<string> GetPermissions(string userId, int companyId)
        {
            return _appUserStore.GetPermissionsAsync(new ApplicationUser { Id = userId }, companyId).Result;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, IEnumerable<string> roles, int companyId)
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

        public IdentityResult AddToRolesByCompany(ApplicationUser user, IEnumerable<string> permissions, int companyId)
        {
            _appUserStore.AddToRolesByCompany(user, permissions, companyId).Wait();
            return IdentityResult.Success;
        }
    }
}
