using FWLog.Data.EnumsAndConsts;
using FWLog.Web.Api.App_Start;
using FWLog.Web.Api.App_Start.NinjectModules;
using log4net;
using Ninject;
using Ninject.Web.Common.WebHost;
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
            AutoMapperConfig.RegisterMappings();
        }

        protected override IKernel CreateKernel()
        {
            return new StandardKernel(new GeneralModule());
        }
    }
}
