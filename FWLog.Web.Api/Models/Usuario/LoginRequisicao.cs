using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Usuario
{
    public class LoginRequisicao
    {
        [StringLength(maximumLength: 5, MinimumLength = 5, ErrorMessage = "O código deve ter 5 dígitos.")]
        public string Codigo { get; set; }
        [Required(ErrorMessage = "Informe a senha.")]
        public string Senha { get; set; }
    }
}