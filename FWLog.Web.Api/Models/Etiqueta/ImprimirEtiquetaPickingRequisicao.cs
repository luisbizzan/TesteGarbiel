using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Etiqueta
{
    public class ImprimirEtiquetaPickingRequisicao
    {
        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }

        [Required(ErrorMessage = "O produto deve ser informado.")]
        public long IdProduto { get; set; }

        [Required(ErrorMessage = "A impressora deve ser informada.")]
        public long IdImpressora { get; set; }
    }
}