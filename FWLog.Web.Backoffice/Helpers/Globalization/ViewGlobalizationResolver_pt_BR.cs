using System.Globalization;
using System.Web;

namespace FWLog.Web.Backoffice.Helpers.Globalization
{
    public class ViewGlobalizationResolver_pt_BR : ViewGlobalizationResolver
    {
        public ViewGlobalizationResolver_pt_BR() : base(new CultureInfo("pt-BR"))
        {

        }

        public override string GetDataTableLanguageUrl()
        {
            string path = VirtualPathUtility.ToAbsolute("~/Scripts/vendors/datatables/languages/datatables-pt-br.json");
            return path;
        }

        public override string GetDecimalNumberMask()
        {
            return "000.000.000.000.000,00";
        }

        public override HtmlString RenderBootstrapSelectScript()
        {
            string path = VirtualPathUtility.ToAbsolute("~/Scripts/vendors/bootstrap-select/i18n/defaults-pt_BR.js");
            return new HtmlString(string.Format(@"<script src=""{0}""></script>", path));
        }

        public override HtmlString RenderMomentLocaleScript()
        {
            string path = VirtualPathUtility.ToAbsolute("~/Scripts/vendors/moment/locale/pt-br.js");
            return new HtmlString(string.Format(@"<script src=""{0}""></script>", path));
        }
    }
}