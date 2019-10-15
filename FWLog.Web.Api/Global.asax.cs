using FWLog.AspNet.Identity;
using FWLog.Data.EnumsAndConsts;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.App_Start.NinjectModules;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Ninject;
using Ninject.Web.Common.WebHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace FWLog.Web.Api
{
    public class MvcApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();
            log4net.Config.XmlConfigurator.Configure();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalContext.Properties["idApplication"] = ApplicationEnum.Api.GetHashCode();
            DependencyResolver.SetResolver(new NinjectDependencyResolver());
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel(new GeneralModule());
            return kernel;
        }
    }
}
