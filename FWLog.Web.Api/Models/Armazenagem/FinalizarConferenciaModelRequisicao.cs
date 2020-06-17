using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class FinalizarConferenciaModelRequisicao
    {
        [Required(ErrorMessage = "O IdEnderecoArmazenagem deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }

        [Required(ErrorMessage = "O IdProduto deve ser informado.")]
        public long IdProduto { get; set; }

        [Required(ErrorMessage = "IdLote deve ser informado.")]
        public long IdLote { get; set; }

        [Required(ErrorMessage = "A Quantidade deve ser informada.")]
        public int Quantidade { get; set; }

        public bool ConferenciaManual { get; set; }
    }
}