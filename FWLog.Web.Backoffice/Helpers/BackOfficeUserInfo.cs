using FWLog.Data.Logging;
using Microsoft.AspNet.Identity;
using System.Web;

namespace FWLog.Web.Backoffice.Helpers
{
    public class BackOfficeUserInfo : IBackOfficeUserInfo
    {
        private object _userIdCache = null;

        public bool IsAuthenticated
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated; }
        }

        /// <summary>
        /// Returns the UserId.
        /// </summary>
        public object UserId
        {
            get
            {
                if (_userIdCache == null)
                {
                    _userIdCache = GetUserIdWithoutCaching();
                }

                return _userIdCache;
            }
        }

        public string IP
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }

                return HttpContext.Current.Request.UserHostAddress;
            }
        }

        private object GetUserIdWithoutCaching()
        {
            if (HttpContext.Current == null || !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return null;
            }

            try
            {
                return HttpContext.Current.User.Identity.GetUserId();
            }
            catch
            {
                return null;
            }
        }
    }
}