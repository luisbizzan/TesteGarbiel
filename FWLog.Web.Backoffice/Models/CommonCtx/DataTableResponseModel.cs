using Newtonsoft.Json;
using System.Collections;

namespace FWLog.Web.Backoffice.Models.CommonCtx
{
    public class DataTableResponseModel
    {
        [JsonProperty("draw")]
        public int Draw { get; set; }

        [JsonProperty("recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonProperty("recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonProperty("data")]
        public IEnumerable Data { get; set; }
    }
}