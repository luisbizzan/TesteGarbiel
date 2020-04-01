using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Etiqueta
{
    public class ImprimirEtiquetaLoteRequisicao
    {
        [Required(ErrorMessage = "O lote deve ser informado.")]
        public long IdLote { get; set; }

        [Required(ErrorMessage = "O produto deve ser informado.")]
        public long IdProduto { get; set; }

        [Required(ErrorMessage = "A impressora deve ser informada.")]
        public long IdImpressora { get; set; }

        [Required(ErrorMessage = "A quantidade de produtos deve ser informada.")]
        public int QuantidadeProdutos { get; set; }

        [Required(ErrorMessage = "A quantidade de etiquetas deve ser informada.")]
        public int QuantidadeEtiquetas { get; set; }
    }
}