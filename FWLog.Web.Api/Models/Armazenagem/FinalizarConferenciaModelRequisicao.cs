using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class FinalizarConferenciaModelRequisicao
    {
        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }

        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdProduto { get; set; }

        [Required(ErrorMessage = "A quantidade deve ser informada.")]
        public int Quantidade { get; set; }

        public bool ConferenciaManual { get; set; }
    }
}