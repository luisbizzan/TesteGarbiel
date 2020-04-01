using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Usuario
{
    public class ValidarUsuarioRequisicao
    {
        [StringLength(maximumLength: 5, MinimumLength = 5, ErrorMessage = "O código deve ter 5 dígitos.")]
        public string Codigo { get; set; }
    }
}