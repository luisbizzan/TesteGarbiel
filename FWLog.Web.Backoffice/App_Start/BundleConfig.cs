using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace FWLog.Web.Backoffice
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterScripts(bundles);
            RegisterStyles(bundles);
            RegisterViewScriptsBundle(bundles);
            RegisterViewStylesBundle(bundles);

            // Necessário configurar o orderer de cada bundle para que a ordem definida seja seguida durante o carregamento.
            foreach (var bundle in bundles.GetRegisteredBundles())
            {
                bundle.Orderer = new AsIsBundleOrderer();
            }

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif

        }

        private static void RegisterScripts(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/script-bundles/bootstrap").Include(
                "~/Scripts/bootstrap/bootstrap.js",
                "~/Scripts/gentelella/helpers/smartresize.js",
                "~/Scripts/gentelella/custom.js"
            ));

            bundles.Add(new ScriptBundle("~/script-bundles/jquery").Include(
                "~/Scripts/jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/script-bundles/jqueryval").Include(
                "~/Scripts/jqueryval/jquery.validate.js",
                "~/Scripts/jqueryval/jquery.validate.unobtrusive.js",
                "~/Scripts/jqueryval/jquery.unobtrusive-ajax.js",
                "~/Scripts/jqueryval/jquery.validate.unobtrusive.bootstrap.js"
            ));

            // PNotify
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/pnotify").Include(
               "~/Scripts/vendors/pnotify/pnotify.custom.min.js"
            ));

            // DataTables
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/datatables").Include(
                "~/Scripts/vendors/datatables/jquery.dataTables.js",
                "~/Scripts/vendors/datatables/dataTables.responsive.js",
                "~/Scripts/vendors/datatables/dataTables.scroller.js",
                "~/Scripts/vendors/datatables/dataTables.buttons.js",
                "~/Scripts/vendors/datatables/dataTables.select.min.js",
                "~/Scripts/vendors/datatables/dataTables.bootstrap.js",
                "~/Scripts/vendors/datatables/responsive.bootstrap.js",
                "~/Scripts/vendors/datatables/buttons.bootstrap.js"
            ));

            bundles.Add(new ScriptBundle("~/script-bundles/vendors/iCheck").Include(
                "~/Scripts/vendors/iCheck/icheck.js"
            ));

            bundles.Add(new ScriptBundle("~/script-bundles/vendors/mask").Include(
                "~/Scripts/vendors/mask/jquery.mask.min.js"
            ));

            bundles.Add(new ScriptBundle("~/script-bundles/vendors/dropzone").Include(
                "~/Scripts/vendors/dropzone/dropzone.js"
            ));

            bundles.Add(new ScriptBundle("~/script-bundles/dart").Include(
                "~/Scripts/dart/*.js"
            ));

            // Moment
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/moment").Include(
                "~/Scripts/vendors/moment/moment.js"
            ));

            // Datetimepicker
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/datetimepicker").Include(
                "~/Scripts/vendors/datetimepicker/bootstrap-datetimepicker.min.js"
            ));

            // NProgress
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/nprogress").Include(
                "~/Scripts/vendors/nprogress/nprogress.js"
            ));

            // bootstrap-select
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/bootstrap-select").Include(
                "~/Scripts/vendors/bootstrap-select/bootstrap-select.js"
            ));

            // Cropper
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/cropper").Include(
                "~/Scripts/vendors/croppper/cropper.js"
            ));

            // Devbridge AutoComplete
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/autocomplete").Include(
                "~/Scripts/vendors/devbridge-autocomplete/jquery.autocomplete.js"
            ));

            // OnScan
            bundles.Add(new ScriptBundle("~/script-bundles/vendors/onscan").Include(
               "~/Scripts/vendors/onScan/onscan.min.js"
            ));
        }

        private static void RegisterStyles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/style-bundles/bootstrap")
                .Include("~/Content/font-awesome/font-awesome.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/bootstrap/bootstrap.min.css", new CssRewriteUrlTransformWrapper())
                .Include(
                    "~/Content/gentelella/custom.css",
                    "~/Content/dart/dart-template.css",
                    "~/Content/site.css"
                ));

            bundles.Add(new StyleBundle("~/style-bundles/vendors/pnotify").Include(
                 "~/Content/vendors/pnotify/pnotify.custom.min.css"
            ));

            bundles.Add(new StyleBundle("~/style-bundles/vendors/datatables").Include(
                "~/Content/vendors/datatables/dataTables.bootstrap.css",
                "~/Content/vendors/datatables/responsive.bootstrap.css",
                "~/Content/vendors/datatables/scroller.bootstrap.css",
                "~/Content/vendors/datatables/buttons.dataTables.css",
                "~/Content/vendors/datatables/buttons.bootstrap.css"));

            bundles.Add(new StyleBundle("~/style-bundles/vendors/iCheck").Include(
                "~/Content/vendors/iCheck/green.css", new CssRewriteUrlTransformWrapper())
            );

            // Datetimepicker
            bundles.Add(new StyleBundle("~/style-bundles/vendors/datetimepicker").Include(
                "~/Content/vendors/datetimepicker/bootstrap-datetimepicker.css"
            ));

            // dropzone
            bundles.Add(new StyleBundle("~/style-bundles/vendors/dropzone").Include(
                "~/Content/vendors/dropzone/dropzone.css",
                "~/Content/vendors/dropzone/basic.css"
            ));

            // NProgress
            bundles.Add(new StyleBundle("~/style-bundles/vendors/nprogress").Include(
                "~/Content/vendors/nprogress/nprogress.css"
            ));

            // bootstrap-select
            bundles.Add(new StyleBundle("~/style-bundles/vendors/bootstrap-select").Include(
                "~/Content/vendors/bootstrap-select/bootstrap-select.css"
            ));

            // Cropper
            bundles.Add(new StyleBundle("~/style-bundles/vendors/cropper").Include(
               "~/Content/vendors/croppper/cropper.css"
            ));
        }

        private static void RegisterViewScriptsBundle(BundleCollection bundles, string pathViews = "~/Scripts/Views/")
        {
            var directories = GetPathDirectories(pathViews);

            if (directories != null)
            {
                foreach (var d in directories)
                {
                    var files = d.GetFiles();

                    foreach (var f in files)
                    {
                        var bundlePath = "~/script-bundles/views/" + f.Directory.Name + "/" + f.Name.Replace(f.Extension, string.Empty);
                        var filePath = pathViews + f.Directory.Name + "/" + f.Name;
                        bundles.Add(new ScriptBundle(bundlePath).Include(
                            filePath
                        ));
                    }
                }
            }
        }

        private static void RegisterViewStylesBundle(BundleCollection bundles, string pathViews = "~/Content/Views/")
        {
            var directories = GetPathDirectories(pathViews);

            if (directories != null)
            {
                foreach (var d in directories)
                {
                    var files = d.GetFiles();

                    foreach (var f in files)
                    {
                        var bundlePath = "~/style-bundles/views/" + f.Directory.Name + "/" + f.Name.Replace(f.Extension, string.Empty);
                        var filePath = pathViews + f.Directory.Name + "/" + f.Name;
                        bundles.Add(new StyleBundle(bundlePath).Include(
                            filePath
                        ));
                    }
                }
            }
        }

        private static DirectoryInfo[] GetPathDirectories(string pathViews)
        {
            var path = HttpContext.Current.Server.MapPath(pathViews);

            if (!string.IsNullOrEmpty(path))
            {
                var viewsInfo = new DirectoryInfo(path);

                var directories = viewsInfo != null && viewsInfo.Exists ? viewsInfo.GetDirectories() : null;

                return directories;
            }

            return null;
        }
    }


    class AsIsBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }

    /// <summary>
    /// A CssRewriteUrlTransform that adds the server virtual path if necessary.
    /// </summary>
    class CssRewriteUrlTransformWrapper : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            return new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
        }
    }
}