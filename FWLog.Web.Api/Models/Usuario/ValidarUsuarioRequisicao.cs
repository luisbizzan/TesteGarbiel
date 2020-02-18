using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Usuario
{
    public class ValidarUsuarioRequisicao
    {
        [Required(ErrorMessage = "Informe o código do usuário.")]
        [StringLength(maximumLength: 5, MinimumLength = 5, ErrorMessage = "O código do usuario deve ter 5 números.")]
        public string Codigo { get; set; }
    }
}