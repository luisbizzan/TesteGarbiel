using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity
{

    public class ApplicationRoleStore : IQueryableRoleStore<ApplicationRole>, IQueryableRoleStore<ApplicationRole, string>, IRoleStore<ApplicationRole, string>, IDisposable
    {
        public IQueryable<ApplicationRole> Roles => this.db.Roles.Where(x => x.ApplicationId == this.appId);

        private readonly ApplicationDbContext db;
        private int appId;

        public ApplicationRoleStore(ApplicationDbContext db, int appId)
        {
            this.db = db;
            this.appId = appId;
        }

        // IRoleStore<TKey> implementation        
        public Task CreateAsync(ApplicationRole role)
        {
            if (role == null)
            {
                throw new ArgumentException(nameof(role));
            }

            role.ApplicationId = this.appId;

            this.db.Roles.Add(role);
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(ApplicationRole role)
        {
            if (role == null)
            {
                throw new ArgumentException(nameof(role));
            }

            if (role.ApplicationId != this.appId)
            {
                throw new InvalidOperationException("This role do not belong to the current application.");
            }

            var dbEntity = this.db.Roles
                .Include(x => x.RolePermissions)
                .Include(x => x.Users)
                .First(x => x.Id == role.Id);

            dbEntity.RolePermissions.Clear();
            dbEntity.Users.Clear();

            this.db.Roles.Remove(dbEntity);
            return this.db.SaveChangesAsync();
        }

        public Task<ApplicationRole> FindByIdAsync(string roleId)
        {
            return this.db.Roles
                .Include(x => x.RolePermissions).Include(x => x.RolePermissions.Select(y => y.Permission))
                .FirstOrDefaultAsync(x => x.ApplicationId == appId && x.Id == roleId);
        }

        public Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            return this.db.Roles
                .Include(x => x.RolePermissions).Include(x => x.RolePermissions.Select(y => y.Permission))
                .FirstOrDefaultAsync(x => x.ApplicationId == appId && x.Name == roleName);
        }

        public Task UpdateAsync(ApplicationRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.ApplicationId = this.appId;
            this.db.Entry(role).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        // IDisposable implementation

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.db != null)
            {
                this.db.Dispose();
            }
        }

        // Custom implementation

        private IList<string> GetPermissionIdsFromNames(IEnumerable<string> permissionNames)
        {
            var permissions = this.db.Permissions.Where(x => x.ApplicationId == this.appId).ToList();

            IList<string> ids = permissions
                .Where(x => permissionNames.Contains(x.Name))
                .Select(x => x.Id)
                .ToList();

            return ids;
        }

        public Task<IList<string>> GetPermissionsAsync(ApplicationRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            IList<string> permissions = db.Roles
                .Where(x => x.Id == role.Id && x.ApplicationId == this.appId)
                .SelectMany(x => x.RolePermissions.Select(y => y.Permission.Name))
                .ToList();

            return Task.FromResult(permissions);
        }

        public Task UpdateAsync(ApplicationRole role, IEnumerable<string> permissions)
        {
            if (role == null)
            {
                throw new ArgumentException(nameof(role));
            }

            ValidatePermissions(permissions);
            role.ApplicationId = this.appId;

            ApplicationRole dbEntity = this.db.Roles.Include(w => w.RolePermissions).FirstOrDefault(x => x.Id == role.Id);

            this.db.Entry(dbEntity).CurrentValues.SetValues(role);

            var permissionIds = GetPermissionIdsFromNames(permissions);

            List<RolePermission> rolePermissions =
                permissionIds.Select(x => new RolePermission
                {
                    PermissionId = x,
                    RoleId = dbEntity.Id,
                })
                .ToList();

            dbEntity.RolePermissions = rolePermissions;

            return this.db.SaveChangesAsync();
        }

        public Task CreateAsync(ApplicationRole role, IEnumerable<string> permissions)
        {
            if (role == null)
            {
                throw new ArgumentException(nameof(role));
            }

            ValidatePermissions(permissions);

            role.ApplicationId = this.appId;
            var permissionsIds = GetPermissionIdsFromNames(permissions);

            List<RolePermission> rolePermissions =
                permissionsIds.Select(x => new RolePermission
                {
                    PermissionId = x
                })
                .ToList();

            role.RolePermissions = rolePermissions;

            this.db.Roles.Add(role);

            return this.db.SaveChangesAsync();
        }

        public Task AddToPermissionsAsync(ApplicationRole role, IEnumerable<string> permissions)
        {
            if (role == null)
            {
                throw new ArgumentException(nameof(role));
            }

            role = this.db.Roles.Include(x => x.RolePermissions).FirstOrDefault(x => x.Id == role.Id);

            IList<ApplicationPermission> appPermisions = this.db.Permissions.Where(x => x.ApplicationId == this.appId).ToList();

            foreach (string permission in permissions)
            {
                ApplicationPermission found = appPermisions.FirstOrDefault(x => x.Name == permission);

                if (found == null)
                {
                    throw new InvalidOperationException("One or more permissions could not be found.");
                }

                if (!role.RolePermissions.Any(x => x.PermissionId == found.Id))
                {
                    role.RolePermissions.Add(new RolePermission { PermissionId = found.Id, RoleId = role.Id });
                }
            }

            return this.db.SaveChangesAsync();
        }

        private void ValidatePermissions(IEnumerable<string> permissions)
        {
            if (!permissions.Any())
            {
                return;
            }

            IList<ApplicationPermission> appPermissions = this.db.Permissions.Where(x => x.ApplicationId == this.appId).ToList();

            if (permissions.Any(x => !appPermissions.Any(y => y.Name == x)))
            {
                throw new ArgumentException("One or more permissions do not belong to the current application.", nameof(permissions));
            }
        }

        public Task CreatePermissionAsync(ApplicationPermission permission)
        {
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission));
            }

            permission.ApplicationId = appId;
            this.db.Permissions.Add(permission);

            return this.db.SaveChangesAsync();
        }

        public Task<ApplicationPermission> FindPermissionByNameAsync(string permissionName)
        {
            return this.db.Permissions
                .FirstOrDefaultAsync(x => x.ApplicationId == this.appId && x.Name == permissionName);
        }


    }
}