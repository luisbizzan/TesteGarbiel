using FWLog.Web.Backoffice.Helpers;
using Newtonsoft.Json;

namespace FWLog.Web.Backoffice.Models.CommonCtx
{
    public class NotifyModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonIgnore]
        public NotifyType Type { get; set; }
    }
}