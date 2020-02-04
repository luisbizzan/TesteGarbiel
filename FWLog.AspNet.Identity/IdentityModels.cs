using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity
{
    [Table("AspNetPermissions", Schema = "DARTQA")]
    public class ApplicationPermission
    {
        [Key]
        public string Id { get; set; }
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public ICollection<RolePermission> Roles { get; set; }
        public ICollection<UserPermission> Users { get; set; }

        public ApplicationPermission() { }

        public ApplicationPermission(string name)
        {
            this.Name = name;
        }
    }

    [Table("AspNetRolePermissions", Schema = "DARTQA")]
    public class RolePermission
    {
        [Key, Column(Order = 0)]
        public string RoleId { get; set; }

        [Key, Column(Order = 1)]
        public string PermissionId { get; set; }

        public virtual ApplicationRole Role { get; set; }
        public virtual ApplicationPermission Permission { get; set; }
    }

    [Table("AspNetUserPermissions", Schema = "DARTQA")]
    public class UserPermission
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        public string PermissionId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationPermission Permission { get; set; }
    }

    [Table("AspNetUsers", Schema = "DARTQA")]
    public class ApplicationUser : IdentityUser<string, IdentityUserLogin, UserRole, IdentityUserClaim>, IUser, IUser<string>
    {
        public int ApplicationId { get; set; }
        public long? IdApplicationSession { get; set; }
        public virtual ICollection<UserPermission> Permissions { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    [Table("AspNetRoles", Schema = "DARTQA")]
    public class ApplicationRole : IdentityRole<string, UserRole>
    {
        public int ApplicationId { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        public ApplicationRole()
        {

        }

        public ApplicationRole(string name)
        {
            this.Name = name;
        }
    }

    [Table("AspNetUserRoles", Schema = "DARTQA")]
    public class UserRole : IdentityUserRole
    {
        public long CompanyId { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, UserRole, IdentityUserClaim>
    {
        public virtual IDbSet<ApplicationPermission> Permissions { get; set; }
        public virtual IDbSet<UserPermission> UserPermissions { get; set; }
        public virtual IDbSet<UserRole> UserRoles { get; set; }

        public ApplicationDbContext()
            : base("IdentityConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("DARTQA");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>().HasKey(k => new { k.UserId, k.RoleId, k.CompanyId });
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            if (entityEntry != null && entityEntry.State == EntityState.Added)
            {
                var errors = new List<DbValidationError>();
                var user = entityEntry.Entity as ApplicationUser;

                if (user != null)
                {
                    if (this.Users.Any(x => x.UserName == user.UserName && x.ApplicationId == user.ApplicationId))
                    {
                        string message = string.Format("Username {0} is already taken for IdApplication {1}", user.UserName, user.ApplicationId);
                        errors.Add(new DbValidationError("User", message));
                    }

                    if (this.RequireUniqueEmail &&
                        this.Users.Any(x => x.Email == user.Email && x.ApplicationId == user.ApplicationId))
                    {
                        string message = string.Format("Email Address {0} is already taken for IdApplication {1}", user.Email, user.ApplicationId);
                        errors.Add(new DbValidationError("User", message));
                    }
                }
                else
                {
                    var role = entityEntry.Entity as ApplicationRole;

                    if (role != null && this.Roles.Any(x => x.Name == role.Name && x.ApplicationId == role.ApplicationId))
                    {
                        string message = string.Format("Role {0} already exists", role.Name);
                        errors.Add(new DbValidationError("Role", message));
                    }
                }
                if (errors.Any())
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }

            return new DbEntityValidationResult(entityEntry, new List<DbValidationError>());
        }
    }
}
