using System;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Optimization;

namespace FWLog.Web.Backoffice.Helpers.Bundling
{
    public class BundleHelper
    {
        private static string scriptsPath = "~/script-bundles/views/";
        private static string stylesPath = "~/style-bundles/views/";

        public static IHtmlString RenderViewScript()
        {
            return RenderViewScript(null);
        }

        public static IHtmlString RenderViewScriptModal()
        {
            return RenderViewScriptModal(null);
        }

        public static IHtmlString RenderViewScript(object viewObj)
        {
            var action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            var controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            string bundlePath = GetViewScriptBundlePath(action, controller);

            if (!BundleTable.Bundles.Any(x => string.Equals(x.Path, bundlePath, StringComparison.InvariantCultureIgnoreCase)))
            {
                return new HtmlString(string.Empty);
            }

            if (viewObj == null)
            {
                return RenderViewScript(action, controller);
            }

            return new HtmlString(GenerateViewObject(viewObj).ToString() + RenderViewScript(action, controller).ToString());
        }

        public static IHtmlString RenderViewScriptModal(object viewObj)
        {
            var action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            var controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            string bundlePath = GetViewScriptBundlePath(action, controller);

            if (!BundleTable.Bundles.Any(x => string.Equals(x.Path, bundlePath, StringComparison.InvariantCultureIgnoreCase)))
            {
                return new HtmlString(string.Empty);
            }

            if (viewObj == null)
            {
                return RenderViewScript(action, controller);
            }

            return new HtmlString(GenerateViewObjectModal(viewObj).ToString() + RenderViewScript(action, controller).ToString());
        }

        public static IHtmlString RenderViewStyle()
        {
            var action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            var controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            string bundlePath = GetViewStyleBundlePath(action, controller);

            if (!BundleTable.Bundles.Any(x => string.Equals(x.Path, bundlePath, StringComparison.InvariantCultureIgnoreCase)))
            {
                return new HtmlString(string.Empty);
            }

            return RenderViewStyle(action, controller);
        }

        private static IHtmlString RenderViewScript(string action, string controller)
        {
            return Scripts.Render(GetViewScriptBundlePath(action, controller));
        }

        private static IHtmlString RenderViewStyle(string action, string controller)
        {
            return Styles.Render(GetViewStyleBundlePath(action, controller));
        }

        private static IHtmlString GenerateViewObject(object obj)
        {
            HtmlString scriptString = new HtmlString(
                "<script>" +
                "var view = " + Json.Encode(obj) + ";" +
                "</script>"
            );

            return scriptString;
        }

        private static IHtmlString GenerateViewObjectModal(object obj)
        {
            HtmlString scriptString = new HtmlString(
                "<script>" +
                 "var view_modal = " + Json.Encode(obj) + ";" +
                "</script>"
            );

            return scriptString;
        }

        private static string GetViewScriptBundlePath(string action, string controller)
        {
            return string.Format("{0}{1}/{2}", scriptsPath, controller, action);
        }

        private static string GetViewStyleBundlePath(string action, string controller)
        {
            return string.Format("{0}{1}/{2}", stylesPath, controller, action);
        }

    }
}