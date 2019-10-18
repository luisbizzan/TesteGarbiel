using Newtonsoft.Json;

namespace FWLog.Services.Model.AccountService
{
    public class TokenErrrorResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string Description { get; set; }
    }
}
