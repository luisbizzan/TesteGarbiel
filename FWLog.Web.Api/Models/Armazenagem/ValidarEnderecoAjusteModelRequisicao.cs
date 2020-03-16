using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class ValidarEnderecoAjusteModelRequisicao
    {
        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }
    }
}