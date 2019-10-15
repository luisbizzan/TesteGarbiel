using log4net;
using System;

namespace FWLog.Web.Api.Helpers
{
    public class LogHelper
    {
        private static readonly ILog _log = (ILog)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(ILog));

        public static void Warn(string message)
        {
            _log.Warn(message);
        }

        public static void Error(Exception ex)
        {
            _log.Error(ex.Message, ex);
        }
    }
}