using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity
{
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IQueryableUserStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>, IUserLoginStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserLockoutStore<ApplicationUser, string>, IUserTwoFactorStore<ApplicationUser, string>, IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly int appId;

        public IQueryable<ApplicationUser> Users => this.db.Users.Where(x => x.ApplicationId == this.appId);

        public ApplicationUserStore(ApplicationDbContext db, int appId)
        {
            this.db = db ?? throw new ArgumentNullException("db");
            this.appId = appId;
        }

        // IUserStore<TUser> implementation

        public Task CreateAsync(ApplicationUser user)
        {
            user.ApplicationId = this.appId;
            this.db.Users.Add(user);
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            if (user.ApplicationId != this.appId)
            {
                throw new InvalidOperationException("Cannot delete used because he doesn't belong to the application.");
            }

            this.db.Users.Remove(user);
            return this.db.SaveChangesAsync();
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return this.db.Users.Include(u => u.Logins).Include(u => u.Roles).Include(u => u.Claims)
                .FirstOrDefaultAsync(u => u.ApplicationId == this.appId && u.Id == userId);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return this.db.Users
                .Include(u => u.Logins).Include(u => u.Roles).Include(u => u.Claims)
                .FirstOrDefaultAsync(u => u.ApplicationId == this.appId && u.UserName == userName);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            user.ApplicationId = this.appId;
            this.db.Entry<ApplicationUser>(user).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // IDisposable implementation

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.db != null)
            {
                this.db.Dispose();
            }
        }

        // IUserRoleStore<TUser> implementation

        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("The property {0} cannot be null or empty.", nameof(roleName));
            }

            var role = this.db.Roles.SingleOrDefault(r => r.Name == roleName && r.ApplicationId == this.appId);

            if (role == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role not found.", new object[] { roleName }));
            }

            user.Roles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });

            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("The property {0} cannot be null or empty.", nameof(roleName));
            }

            var role = this.db.Roles.SingleOrDefault(r => r.Name == roleName && r.ApplicationId == this.appId);

            var userRole = user.Roles.SingleOrDefault(r => r.RoleId == role.Id);

            if (userRole != null)
            {
                user.Roles.Remove(userRole);
            }

            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<string> userRoles = this.db.Roles.Where(x => x.Users.Any(y => y.UserId == user.Id)).Select(x => x.Name).ToList();
            return Task.FromResult(userRoles);
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("The property {0} cannot be null or empty.", nameof(roleName));
            }

            return Task.FromResult(this.db.Roles.Any(r => r.ApplicationId == this.appId && r.Name == roleName && r.Users.Any(u => u.UserId == user.Id)));
        }

        // IUserPasswordStore<TUser> implementation

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }


        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        // IUserSecurityStampStore<TUser> implementation

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp);
        }

        // IUserLoginStore<TUser> implementation

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var userLogin = Activator.CreateInstance<IdentityUserLogin>();
            userLogin.UserId = user.Id;
            userLogin.LoginProvider = login.LoginProvider;
            userLogin.ProviderKey = login.ProviderKey;
            user.Logins.Add(userLogin);

            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;

            var item = user.Logins.SingleOrDefault(l => l.LoginProvider == provider && l.ProviderKey == key);

            if (item != null)
            {
                user.Logins.Remove(item);
            }

            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<UserLoginInfo> infos = user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
            return Task.FromResult(infos);
        }

        public async Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;

            var user = await this.db.Users
                .Include(u => u.Logins).Include(u => u.Roles).Include(u => u.Claims)
                .FirstOrDefaultAsync(x => x.ApplicationId == this.appId && x.Logins.Any(l => l.LoginProvider == provider && l.ProviderKey == key));

            if (user == null)
            {
                return default(ApplicationUser);
            }

            return user;
        }

        // IUserEmailStore<TUser> implementation

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return this.db.Users
                .Include(u => u.Logins).Include(u => u.Roles).Include(u => u.Claims)
                .FirstOrDefaultAsync(u => u.ApplicationId == this.appId && u.Email == email);
        }

        // IUserLockoutStore<TUser, TKey> implementation

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(
                user.LockoutEndDateUtc.HasValue ?
                    new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) :
                    new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? null : new DateTime?(lockoutEnd.UtcDateTime);
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        // IUserTwoFactorStore<TKey> implementation

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        // Custom implementation

        public Task<IList<string>> GetUserRolesByIdEmpresa(ApplicationUser user, long idEmpresa)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<string> roles = db.UserRoles.Where(x => x.UserId == user.Id && x.CompanyId == idEmpresa).Select(s => s.RoleId).ToList();

            IList<string> rolesName = db.Roles.Where(w => roles.Contains(w.Id)).Select(s => s.Name).ToList();

            return Task.FromResult(rolesName);
        }

        public Task<IList<string>> GetPermissionsAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<string> roleIds = db.UserRoles.Where(x => x.UserId == user.Id).Select(s => s.RoleId).ToList();
            IList<string> permissions = db.Users
                .Where(x => x.Id == user.Id && x.ApplicationId == this.appId)
                .SelectMany(x => x.Permissions.Select(y => y.Permission.Name))
                .Union(
                    db.Roles
                    .Where(w => roleIds.Contains(w.Id))
                    .SelectMany(x => x.RolePermissions.Select(y => y.Permission.Name))
                )
                .ToList();

            return Task.FromResult(permissions);
        }

        public Task<IList<string>> GetPermissionsByIdEmpresaAsync(ApplicationUser user, long idEmpresa)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<string> roleIds = db.UserRoles.Where(x => x.UserId == user.Id && x.CompanyId == idEmpresa).Select(s => s.RoleId).ToList();
            IList<string> permissions = db.Users
                .Where(x => x.Id == user.Id && x.ApplicationId == this.appId)
                .SelectMany(x => x.Permissions.Select(y => y.Permission.Name))
                .Union(
                    db.Roles
                    .Where(w => roleIds.Contains(w.Id))
                    .SelectMany(x => x.RolePermissions.Select(y => y.Permission.Name))
                )
                .ToList();

            return Task.FromResult(permissions);
        }

        public Task UpdateAsync(ApplicationUser user, IEnumerable<string> roles, long idEmpresa)
        {
            ValidateRoles(roles);

            user.ApplicationId = this.appId;

            IList<UserRole> userRoles = this.db.Roles
                .Where(x => x.ApplicationId == this.appId && roles.Contains(x.Name))
                .ToList()
                .Select(x => new UserRole { RoleId = x.Id, UserId = user.Id, CompanyId = idEmpresa })
                .ToList();

            var dbEntity = db.UserRoles.Where(x => x.UserId == user.Id && x.CompanyId == idEmpresa).ToList();

            List<UserRole> userRolesToAdd = userRoles.Where(x => !dbEntity.Any(y => y.RoleId == x.RoleId)).ToList();
            List<UserRole> userRolesToRemove = dbEntity.Where(x => !userRoles.Any(y => y.RoleId == x.RoleId)).ToList();

            userRolesToAdd.ForEach(x => db.UserRoles.Add(x));
            userRolesToRemove.ForEach(x => db.UserRoles.Remove(x));

            return this.db.SaveChangesAsync();
        }

        public Task AddToPermissionsAsync(ApplicationUser user, IEnumerable<string> permissions)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<UserPermission> userPermissions = this.db.UserPermissions.Where(x => x.UserId == user.Id).ToList();

            IList<ApplicationPermission> appPermisions = this.db.Permissions.Where(x => x.ApplicationId == this.appId).ToList();

            foreach (string permission in permissions)
            {
                ApplicationPermission found = appPermisions.FirstOrDefault(x => x.Name == permission);

                if (found == null)
                {
                    throw new InvalidOperationException("One or more permissions could not be found.");
                }

                if (!userPermissions.Any(x => x.PermissionId == found.Id))
                {
                    this.db.UserPermissions.Add(new UserPermission { PermissionId = found.Id, UserId = user.Id });
                }
            }

            return this.db.SaveChangesAsync();
        }

        public Task AddToRolesByEmpresa(ApplicationUser user, IEnumerable<string> roles, long idEmpresa)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<UserRole> userRoles = this.db.UserRoles.Where(x => x.UserId == user.Id && x.CompanyId == idEmpresa).ToList();

            IList<ApplicationRole> appRoles = this.db.Roles.Where(x => x.ApplicationId == this.appId).ToList();

            foreach (string permission in roles)
            {
                ApplicationRole found = appRoles.FirstOrDefault(x => x.Name == permission);

                if (found == null)
                {
                    throw new InvalidOperationException("One or more roles could not be found.");
                }

                if (!userRoles.Any(x => x.RoleId == found.Id))
                {
                    this.db.UserRoles.Add(new UserRole { RoleId = found.Id, UserId = user.Id, CompanyId = idEmpresa });
                }
            }

            return this.db.SaveChangesAsync();
        }


        private void ValidateRoles(IEnumerable<string> roles)
        {
            if (!roles.Any())
            {
                return;
            }

            IList<string> appRoles = this.db.Roles.Where(x => x.ApplicationId == this.appId).Select(x => x.Name).ToList();

            if (roles.Any(x => !appRoles.Contains(x)))
            {
                throw new ArgumentException("One or more roles do not belong to the current application.", nameof(roles));
            }
        }
    }
}
;