using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Usuario
{
    public class ValidarPermissaoRequisicao : LoginRequisicao
    {
        [Required(ErrorMessage = "Informe a permissão.")]
        public string Permissao { get; set; }
    }
}