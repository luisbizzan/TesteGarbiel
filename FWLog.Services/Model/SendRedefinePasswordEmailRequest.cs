namespace FWLog.Services.Model.AccountService
{
    public class SendRedefinePasswordEmailRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string UserEmail { get; set; }
    }
}
