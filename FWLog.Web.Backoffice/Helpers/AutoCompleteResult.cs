using FWLog.Web.Backoffice.Models.CommonCtx;
using Newtonsoft.Json;
using System.Text;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Helpers
{
    public class AutoCompleteResult : ContentResult
    {
        private AutoCompleteResult(string content)
        {
            ContentType = "application/json";
            Content = content;
            ContentEncoding = Encoding.UTF8;
        }

        public static AutoCompleteResult FromModel(AutoCompleteResponseModel model)
        {
            string content = JsonConvert.SerializeObject(model);
            return new AutoCompleteResult(content);
        }
    }
}