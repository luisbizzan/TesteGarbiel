using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class ValidarImpressoraRomaneioRequisicao
    {
        [Required(ErrorMessage = "O usuário deve ser informado.")]
        public string IdUsuario { get; set; }
        [Required(ErrorMessage = "A empresa deve ser informado.")]
        public long IdEmpresa { get; set; }
    }
}