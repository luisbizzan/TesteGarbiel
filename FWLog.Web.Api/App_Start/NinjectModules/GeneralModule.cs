using FWLog.Data;
using log4net;
using Ninject.Modules;
using Ninject.Web.Common;

namespace FWLog.Web.Api.App_Start.NinjectModules
{
    public class GeneralModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILog>().ToMethod(ctx => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            Bind<UnitOfWork>().ToSelf().InRequestScope();
        }
    }
}