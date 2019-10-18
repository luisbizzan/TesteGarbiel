using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FWLog.Web.Backoffice.Helpers
{
    public static class HtmlRequestHelper
    {
        public static string Id()
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("id"))
                return (string)routeValues["id"];
            else if (HttpContext.Current.Request.QueryString.AllKeys.Contains("id"))
                return HttpContext.Current.Request.QueryString["id"];

            return string.Empty;
        }

        public static string Controller()
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }

        public static string Action()
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }

        public static string Area()
        {
            var dataTokens = HttpContext.Current.Request.RequestContext.RouteData.DataTokens;

            if (dataTokens.ContainsKey("area"))
                return (string)dataTokens["area"];

            return string.Empty;
        }

        public static string Host()
        {
            var http = HttpContext.Current.Request.Headers["X-Forwarded-Proto"];
            var pathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
            var controllerPositionUrl = pathAndQuery.IndexOf(Controller());
            var subhost = "/";
            if (controllerPositionUrl != -1)
            {
                subhost = pathAndQuery.Remove(controllerPositionUrl);
            }
            var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;

            if (http != null)
            {
                if (http.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    absoluteUri = absoluteUri.Replace("http", "https");
                }
            }
            var host = absoluteUri.Replace(pathAndQuery, subhost);

            return host;
        }

        public static string Http()
        {
            var http = HttpContext.Current.Request.Headers["X-Forwarded-Proto"];
            if (http != null)
            {
                if (http.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    return "https";
                }
            }
            return "http";
        }
    }
}