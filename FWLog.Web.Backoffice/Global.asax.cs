﻿using DartDigital.Library.Web.Globalization;
using DartDigital.Library.Web.IO;
using DartDigital.Library.Web.ModelValidation;
using DartDigital.Library.Web.ModelValidation.Adapters;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.App_Start;
using FWLog.Web.Backoffice.EnumsAndConsts;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Linq;

namespace FWLog.Web.Backoffice
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            var postAuthenticateHandler = new EventHandlerTaskAsyncHelper(PostAuthenticateRequestEvent);
            this.AddOnPostAuthenticateRequestAsync(postAuthenticateHandler.BeginEventHandler, postAuthenticateHandler.EndEventHandler);
        }

        protected void Application_Init()
        {
            var ctx = HttpContext.Current;
        }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DependencyResolver.SetResolver(new NinjectDependencyResolver());

            RegisterLocalizationAdapters();

            TimeZoneInfo brasiliaTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            var timeZoneResolver = new WebAppTimeZoneResolver(brasiliaTimeZoneInfo);
            DateTimeConvert.Initialize(timeZoneResolver);

            WebAppCultureManager.Initialize(new CultureInfo("pt-BR"), new ApplicationCultureProvider());

            AutoMapperConfig.RegisterMappings();
            FileHelper.SetCurrent(AppDomain.CurrentDomain.BaseDirectory);

            // Coluna IdApplication da tabela Application, utilizado no log4net
            GlobalContext.Properties["idApplication"] = ApplicationEnum.BackOffice.GetHashCode().ToString();

            ClientDataTypeModelValidatorProvider.ResourceClassKey = nameof(Resources.ModelValidationStrings);
            DefaultModelBinder.ResourceClassKey = nameof(Resources.ModelValidationStrings);
        }

        private void RegisterLocalizationAdapters()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(LocalizedRequiredAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(StringLengthAttribute), typeof(LocalizedStringLengthAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RangeAttribute), typeof(LocalizedRangeAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcEmailValidationAttribute), typeof(LocalizedMvcEmailValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcUrlValidationAttribute), typeof(LocalizedMvcUrlValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcIPAddressValidationAttribute), typeof(LocalizedMvcIPAddressValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcCpfValidationAttribute), typeof(LocalizedMvcCpfValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcCnpjValidationAttribute), typeof(LocalizedMvcCnpjValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcCpfOrCnpjValidationAttribute), typeof(LocalizedMvcCpfOrCnpjValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcCepValidationAttribute), typeof(LocalizedMvcCepValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcAlphaOnlyValidationAttribute), typeof(LocalizedMvcAlphaOnlyValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MvcBrazilPhoneValidationAttribute), typeof(LocalizedMvcBrazilPhoneValidationAttributeAdapter<Resources.ModelValidationStrings>));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredIfAttribute), typeof(LocalizedRequiredIfAttributeAdapter<Resources.ModelValidationStrings>));
        }

        private void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            HandleExceptionRedirect(exception);

            // Validação colocada porque erros diferente de 5xx não devem ser logados.      
            if (IsServerError(Response.StatusCode))
            {
                try
                {
                    var log = (ILog)DependencyResolver.Current.GetService(typeof(ILog));

                    log.Error(exception.Message, exception);
                }
                catch { }
            }
        }

        private void HandleExceptionRedirect(Exception exception)
        {
            var routeData = new RouteData();

            var controller = "Error";
            var action = "Index";

            Response.StatusCode = 500;

            if (exception is HttpException httpException)
            {
                Response.StatusCode = httpException.GetHttpCode();

                switch (Response.StatusCode)
                {
                    case 403:
                        action = "Forbidden";
                        break;
                    case 404:
                        action = "NotFound";
                        break;
                    default:
                        action = "Index";
                        break;
                }
            }

            if (Request.IsLocal)
            {
                return;
            }

            Response.TrySkipIisCustomErrors = true;

            var contexWrapper = new HttpContextWrapper(Context);
            var context = new RequestContext(contexWrapper, routeData);

            routeData.Values.Add("controller", controller);
            routeData.Values.Add("action", action);

            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            IController errorController = factory.CreateController(context, controller);

            Server.ClearError();
            Response.Clear();
            errorController.Execute(context);
        }

        private bool IsServerError(int statusCode)
        {
            return statusCode >= 500 && statusCode < 600;
        }

        private async Task PostAuthenticateRequestEvent(object sender, EventArgs e)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || !(User is ClaimsPrincipal))
            {
                return;
            }

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<BackofficeUserManager>();
            var user = (ClaimsPrincipal)User;
            var applicationUser = userManager.FindById(User.Identity.GetUserId());
            
            long idEmpresa = 0;
            if (applicationUser != null)
            {
                var uow = (UnitOfWork)DependencyResolver.Current.GetService(typeof(UnitOfWork));
                ApplicationSession applicationSession;

                if (!applicationUser.IdApplicationSession.HasValue)
                {
                    PerfilUsuario perfilUsuario = uow.PerfilUsuarioRepository.GetByUserId(applicationUser.Id);
                    long idEmpresaPrincipal = perfilUsuario.EmpresaPrincipal.IdEmpresa;

                    applicationSession = new ApplicationSession
                    {
                        IdAspNetUsers = applicationUser.Id,
                        IdApplication = applicationUser.ApplicationId,
                        DataLogin = DateTime.Now,
                        DataUltimaAcao = DateTime.Now,
                        IdEmpresa = idEmpresaPrincipal
                    };

                    uow.ApplicationSessionRepository.Add(applicationSession);
                    uow.SaveChanges();

                    applicationUser.IdApplicationSession = applicationSession.IdApplicationSession;

                    userManager.Update(applicationUser);
                }
                else
                {
                    applicationSession = uow.ApplicationSessionRepository.GetById(applicationUser.IdApplicationSession.Value);
                }                
                
                if (applicationSession != null && applicationSession.IdEmpresa.HasValue)
                {
                    idEmpresa = applicationSession.IdEmpresa.Value;
                }
            }

            string userId = user.Identity.GetUserId();
            IList<string> permissions = await userManager.GetPermissionsByIdEmpresaAsync(userId, idEmpresa).ConfigureAwait(false);
            HttpContext.Current.User = new ApplicationClaimsPrincipal(user, permissions);
        }
    }
}
