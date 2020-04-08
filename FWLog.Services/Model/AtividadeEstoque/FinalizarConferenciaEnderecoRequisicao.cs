using System.ComponentModel.DataAnnotations;

namespace FWLog.Services.Model.AtividadeEstoque
{
    public class FinalizarConferenciaEnderecoRequisicao
    {
        [Required(ErrorMessage = "O produto deve ser informado.")]
        public long IdProduto { get; set; }

        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }

        [Required(ErrorMessage = "A atividade deve ser informada.")]
        public long IdAtividadeEstoque { get; set; }

        public long IdLote { get; set; }

        [Required(ErrorMessage = "A quantidade final deve ser informada.")]
        public int QuantidadeFinal { get; set; }
    }
}