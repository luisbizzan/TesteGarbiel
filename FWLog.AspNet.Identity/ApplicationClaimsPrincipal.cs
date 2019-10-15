using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity
{
    public class ApplicationClaimsPrincipal : ClaimsPrincipal
    {
        public IEnumerable<string> Permissions { get; }

        public ApplicationClaimsPrincipal(ClaimsPrincipal principal, IEnumerable<string> permissions) : base(principal)
        {
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        public bool HasPermission(string permissionName)
        {
            return Permissions.Contains(permissionName);
        }
    }
}
