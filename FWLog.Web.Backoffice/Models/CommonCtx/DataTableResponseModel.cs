using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public IEnumerable<object> Data { get; set; }
    }
}