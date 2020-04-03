using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.AtividadeEstoque
{
    public class ValidarEnderecoConferenciaProdutoForaLinhaRequisicao
    {
        [Required(ErrorMessage = "O corredor deve ser informado.")]
        public int Corredor { get; set; }

        [Required(ErrorMessage = "O produto deve ser informado.")]
        public long IdProduto { get; set; }

        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }
    }
}