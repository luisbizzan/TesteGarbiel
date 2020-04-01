using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Etiqueta
{
    public class ImprimirEtiquetaProdutoRequisicao
    {

        [Required(ErrorMessage = "O produto deve ser informado.")]
        public string ReferenciaProduto { get; set; }

        [Required(ErrorMessage = "A impressora deve ser informada.")]
        public int IdImpressora { get; set; }

        [Required(ErrorMessage = "Tipo da impressão deve ser informado.")]
        public int IdImpressaoItem { get; set; }

        public int QuantidadeEtiquetas { get; set; } = 1;
    }
}