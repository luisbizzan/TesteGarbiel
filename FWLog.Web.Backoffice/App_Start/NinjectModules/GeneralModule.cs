using FWLog.Data;
using FWLog.Data.Logging;
using FWLog.Services.Interfaces;
using FWLog.Web.Backoffice.Helpers;
using log4net;
using Ninject.Modules;
using Ninject.Web.Common;

namespace FWLog.Web.Backoffice.App_Start.NinjectModules
{
    public class GeneralModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAuditLog>().To<BackOfficeAuditLog>();
            Bind<IBackOfficeUserInfo>().To<BackOfficeUserInfo>();
            Bind<ILog>().ToMethod(ctx => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            Bind<IBOAccountContentProvider>().To<BOAccountContentProvider>();

            Bind<UnitOfWork>().ToSelf().InRequestScope();
        }

    }
}