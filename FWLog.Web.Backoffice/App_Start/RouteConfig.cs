﻿using System.Web.Mvc;
using System.Web.Routing;

namespace FWLog.Web.Backoffice
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");

            routes.MapRoute(
                name: "Default", // Route name
                url: "{controller}/{action}/{id}",// URL with parameters
                defaults: new { controller = "BOHome", action = "Index", id = UrlParameter.Optional }// Parameter defaults
            );
        }
    }
}