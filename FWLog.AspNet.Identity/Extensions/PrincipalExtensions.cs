using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.AspNet.Identity.Extensions
{
    public static class PrincipalExtensions
    {
        public static bool HasPermission(this IPrincipal principal, string permissionName)
        {
            if (principal is ApplicationClaimsPrincipal)
            {
                return (principal as ApplicationClaimsPrincipal).HasPermission(permissionName);
            }

            return false;
        }
    }
}
