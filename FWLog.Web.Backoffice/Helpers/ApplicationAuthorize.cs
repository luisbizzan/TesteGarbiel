using FWLog.AspNet.Identity;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Helpers
{
    public sealed class ApplicationAuthorize : AuthorizeAttribute
    {
        public string Permissions { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorized = base.AuthorizeCore(httpContext);

            if (!isAuthorized)
            {
                return false;
            }

            return AuthorizeValidationHelper.UserHasPermission(httpContext, Permissions);
        }
    }
}