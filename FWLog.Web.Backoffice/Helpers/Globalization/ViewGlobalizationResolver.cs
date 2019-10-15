using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Globalization;
using DartDigital.Library.Helpers;
using System.Reflection;
using DartDigital.Library.Web.Globalization;

namespace FWLog.Web.Backoffice.Helpers.Globalization
{
    public abstract class ViewGlobalizationResolver
    {
        private static IEnumerable<ViewGlobalizationResolver> _registeredResolvers;
        private static ViewGlobalizationResolver _defaultResolver;
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Retorna a URL do script que contém a linguage do DataTables.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDataTableLanguageUrl();


        /// <summary>
        /// Renderiza o script que carrega a localização do moment.
        /// </summary>
        /// <returns></returns>
        public abstract HtmlString RenderMomentLocaleScript();

        /// <summary>
        /// Retorna a máscara que é utilizada no jQuery Mask.
        /// </summary>
        public abstract string GetDecimalNumberMask();

        /// <summary>
        /// Renderiza o script que carrega a localização do bootstrap select.
        /// </summary>
        /// <returns></returns>
        public abstract HtmlString RenderBootstrapSelectScript();

        public static ViewGlobalizationResolver GetResolver()
        {
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            ViewGlobalizationResolver resolver = _registeredResolvers.FirstOrDefault(x => x.Culture.Equals(culture));

            if (resolver != null)
            {
                return resolver;
            }

            CultureInfo neutralCulture = CultureHelper.GetNeutralCulture(culture);
            ViewGlobalizationResolver neutralResolver = _registeredResolvers.FirstOrDefault(x => x.Culture.Equals(neutralCulture));

            if (neutralResolver != null)
            {
                return neutralResolver;
            }

            return _defaultResolver;
        }

        static ViewGlobalizationResolver()
        {
            _registeredResolvers = LoadGlobalizationResolversFromAssembly();

            if (!_registeredResolvers.Any())
            {
                _defaultResolver = null;
                return;
            }
            else
            {
                CultureInfo defaultCulture = WebAppCultureManager.Current.DefaultCulture;
                ViewGlobalizationResolver defaultResolver = _registeredResolvers.FirstOrDefault(x => x.Culture.Equals(defaultCulture));

                if (defaultResolver == null)
                {
                    throw new Exception(String.Format("The ViewGlobalizationResolver for the default culture {0} needs to be implemented.", defaultCulture.Name));
                }
            }
        }

        static IEnumerable<ViewGlobalizationResolver> LoadGlobalizationResolversFromAssembly()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.IsSubclassOf(typeof(ViewGlobalizationResolver)));

            var allResolversList = new List<ViewGlobalizationResolver>();

            foreach (var type in types)
            {
                ViewGlobalizationResolver instance = (ViewGlobalizationResolver)Activator.CreateInstance(type, true);
                allResolversList.Add(instance);
            }

            return allResolversList;
        }

        protected ViewGlobalizationResolver(CultureInfo culture)
        {
            Culture = culture;
        }
    }
}