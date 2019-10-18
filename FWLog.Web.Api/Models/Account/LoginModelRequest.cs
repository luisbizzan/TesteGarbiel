using FWLog.Web.Api.GlobalResources;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Account
{
    public class LoginModelRequest
    {
        [Required(ErrorMessageResourceName = nameof(AccountResource.UserNameRequired), ErrorMessageResourceType = typeof(AccountResource))]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceName = nameof(AccountResource.PasswordRequired), ErrorMessageResourceType = typeof(AccountResource))]
        public string Password { get; set; }
        public int CompanyId { get; set; }
    }
}