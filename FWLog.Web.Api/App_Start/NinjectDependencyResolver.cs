using Ninject;
using FWLog.Web.Api.App_Start.NinjectModules;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Api
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel _kernel;

        public NinjectDependencyResolver()
        {
            _kernel = new StandardKernel(new GeneralModule());
            _kernel.Unbind<ModelValidatorProvider>();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
    }
}