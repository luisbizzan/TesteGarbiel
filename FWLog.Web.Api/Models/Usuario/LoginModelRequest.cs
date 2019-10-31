using FWLog.Web.Api.GlobalResources;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Usuario
{
    public class LoginModelRequest
    {
        [Required(ErrorMessageResourceName = nameof(AccountResource.UserNameRequired), ErrorMessageResourceType = typeof(AccountResource))]
        public string Usuario { get; set; }
        [Required(ErrorMessageResourceName = nameof(AccountResource.PasswordRequired), ErrorMessageResourceType = typeof(AccountResource))]
        public string Senha { get; set; }
    }
}