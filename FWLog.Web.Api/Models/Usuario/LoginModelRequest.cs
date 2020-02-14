using FWLog.Web.Api.GlobalResources;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Usuario
{
    public class LoginModelRequest
    {
        [StringLength(maximumLength: 5, MinimumLength = 5, ErrorMessage = "O código do usuário deve ter 5 dígitos.")]
        public string Usuario { get; set; }
        [Required(ErrorMessageResourceName = nameof(AccountResource.PasswordRequired), ErrorMessageResourceType = typeof(AccountResource))]
        public string Senha { get; set; }
    }
}