using FWLog.Web.Backoffice.Models.CommonCtx;
using Newtonsoft.Json;
using System.Text;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Helpers
{
    public class DataTableResult : ContentResult
    {
        private DataTableResult(string content)
        {
            ContentType = "application/json";
            Content = content;
            ContentEncoding = Encoding.UTF8;
        }

        public static DataTableResult FromModel(DataTableResponseModel model)
        {
            string content = JsonConvert.SerializeObject(model);
            return new DataTableResult(content);
        }
    }
}