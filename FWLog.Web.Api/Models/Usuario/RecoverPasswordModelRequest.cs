using FWLog.Web.Api.GlobalResources;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Usuario
{
    public class RecoverPasswordModelRequest
    {
        [Required(ErrorMessageResourceName = nameof(AccountResource.EmailRequired), ErrorMessageResourceType = typeof(AccountResource))]
        public string Email { get; set; }
    }
}