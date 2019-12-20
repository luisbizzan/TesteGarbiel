
using FWLog.AspNet.Identity.Building;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.AspNet.Identity
{
    public class PermissionManager
    {
        public static IEnumerable<PermissionGroupBuildItem> Groups { get; private set; }

        static PermissionManager()
        {
            Groups = new List<PermissionGroupBuildItem>();
        }

        /// <summary>
        /// Cria todas as permissões do builder caso não existam.
        /// </summary>
        public static void CreatePermissionsIfNotExists(PermissionBuilder builder, ApplicationRoleManager roleManager)
        {
            Groups = builder.Groups;

            foreach (PermissionGroupBuildItem group in Groups)
            {
                foreach (Building.PermissionBuildItem permission in group.Permissions)
                {
                    CreatePermissionIfNotExists(permission, roleManager);
                }
            }

        }

        /// <summary>
        /// Cria a role se não existir e víncula as permissões a ela caso não estejam vinculadas.
        /// </summary>
        public static void CreateRoleIfNotExists(ApplicationRole role, IEnumerable<string> permissions, ApplicationRoleManager roleManager)
        {
            ApplicationRole existingRole = roleManager.FindByName(role.Name);

            if (existingRole != null)
            {
                role = existingRole;
            }
            else
            {
                var result = roleManager.Create(role);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(result.Errors.FirstOrDefault());
                }
            }

            AddPermissionsToRoleIfNotExists(role, permissions, roleManager);
        }


        /// <summary>
        /// Cria o usuário se não existir e víncula as roles a ele se não estiverem vinculadas.
        /// </summary>
        public static void CreateUserIfNotExists(ApplicationUser user, string password, IEnumerable<string> roles, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            ApplicationUser existingUser = userManager.FindByName(user.UserName);

            if (existingUser != null)
            {
                user = existingUser;
            }
            else
            {
                user.Id = Guid.NewGuid().ToString();
                var result = userManager.Create(user, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(result.Errors.FirstOrDefault());
                }
            }

            AddRolesToUserIfNotExists(user, roles, userManager, roleManager);
        }

        /// <summary>
        /// Cria o usuário se não existir e víncula a role a ele se não estiver vinculada.
        /// </summary>
        public static void CreateUserIfNotExists(ApplicationUser user, string password, string role, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            CreateUserIfNotExists(user, password, new string[] { role }, userManager, roleManager);
        }

        private static void AddRolesToUserIfNotExists(ApplicationUser user, IEnumerable<string> roles, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            List<string> existingRoles = new List<string>();

            foreach (var userRole in user.Roles)
            {
                string roleName = roleManager.FindById(userRole.RoleId).Name;
                existingRoles.Add(roleName);
            }

            string[] newRoles = roles.Where(x => !existingRoles.Contains(x)).ToArray();
            var result = userManager.AddToRoles(user.Id, newRoles);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.FirstOrDefault());
            }
        }

        private static void AddPermissionsToRoleIfNotExists(ApplicationRole role, IEnumerable<string> permissions, ApplicationRoleManager roleManager)
        {
            IEnumerable<string> existingPermissions = roleManager.GetPermissions(role);
            IEnumerable<string> newPermissions = permissions.Where(x => !existingPermissions.Contains(x));

            var result = roleManager.AddToPermissions(role, newPermissions);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.FirstOrDefault());
            }
        }

        private static void CreatePermissionIfNotExists(Building.PermissionBuildItem permission, ApplicationRoleManager roleManager)
        {
            if (roleManager.FindPermissionByName(permission.Name) == null)
            {
                roleManager.CreatePermission(new ApplicationPermission(permission.Name));
            }
        }
    }
}
