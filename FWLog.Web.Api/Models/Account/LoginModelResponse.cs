namespace FWLog.Web.Api.Models.Account
{
    public class LoginModelResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string ExpiresIn { get; set; }
        public string UserName { get; set; }
        public string Issued { get; set; }
        public string Expires { get; set; }
    }
}