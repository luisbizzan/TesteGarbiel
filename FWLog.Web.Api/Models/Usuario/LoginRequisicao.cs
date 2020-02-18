using FWLog.Web.Api.GlobalResources;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Usuario
{
    public class LoginRequisicao
    {
        [StringLength(maximumLength: 5, MinimumLength = 5, ErrorMessage = "O código do usuario deve ter 5 números.")]
        public string Codigo { get; set; }
        [Required(ErrorMessage = "Informe o código do usuário.")]
        public string Senha { get; set; }
    }
}