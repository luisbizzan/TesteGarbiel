using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.AtividadeEstoque
{
    public class ValidarProdutoConferenciaProdutoForaLinhaRequisicao
    {
        [Required(ErrorMessage = "O corredor deve ser informado.")]
        public int Corredor { get; set; }

        [Required(ErrorMessage = "O produto deve ser informado.")]
        public long IdProduto { get; set; }
    }
}