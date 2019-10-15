using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;

namespace FWLog.AspNet.Identity
{
    public static class AuthorizeValidationHelper
    {
        public static bool UserHasPermission(HttpContextBase httpContext, string permissions)
        {
            return UserHasPermission(httpContext.User, permissions);
        }

        public static bool UserHasPermission(HttpActionContext actionContext, string permissions)
        {
            IPrincipal user = actionContext.ControllerContext.RequestContext.Principal;
            return UserHasPermission(user, permissions);
        }

        private static bool UserHasPermission(IPrincipal user, string permissions)
        {
            if (String.IsNullOrWhiteSpace(permissions))
            {
                return true;
            }

            ApplicationClaimsPrincipal appUser;

            if (!TryGetClaimsPrincipal(user, out appUser))
            {
                return false;
            }

            string[] permissionsSplit = SplitString(permissions);

            if (permissionsSplit.Length > 0 && !permissionsSplit.Any(appUser.HasPermission))
            {
                return false;
            }

            return true;
        }

        private static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;

            return split.ToArray();
        }

        private static bool TryGetClaimsPrincipal(IPrincipal user, out ApplicationClaimsPrincipal principal)
        {
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated || user is ApplicationClaimsPrincipal == false)
            {
                principal = null;
                return false;
            }

            principal = user as ApplicationClaimsPrincipal;
            return true;
        }
    }
}
