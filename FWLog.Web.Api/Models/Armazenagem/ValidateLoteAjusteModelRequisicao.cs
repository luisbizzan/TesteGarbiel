using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class ValidateLoteAjusteModelRequisicao
    {
        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }

        [Required(ErrorMessage = "O lote deve ser informado.")]
        public long IdLote { get; set; }
    }
}