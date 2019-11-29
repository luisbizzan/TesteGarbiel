using System.Globalization;
using System.Web;

namespace FWLog.Web.Backoffice.Helpers.Globalization
{
    public class ViewGlobalizationResolver_en_US : ViewGlobalizationResolver
    {
        public ViewGlobalizationResolver_en_US() : base(new CultureInfo("en-US"))
        {

        }

        public override string GetDataTableLanguageUrl()
        {
            return "false";
        }

        public override string GetDecimalNumberMask()
        {
            return "000,000,000,000,000.00";
        }

        public override HtmlString RenderBootstrapSelectScript()
        {
            // Para inglês não é necessário carregar um script.
            return new HtmlString(string.Empty);
        }

        public override HtmlString RenderMomentLocaleScript()
        {
            // Para inglês não é necessário carregar um script.
            return new HtmlString(string.Empty);
        }
    }
}