using FWLog.Services.Model.Etiquetas;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Etiqueta
{
    public class ImprimirEtiquetaEnderecoRequisicao
    {
        [Required(ErrorMessage = "O endereço deve ser informado.")]
        public long IdEnderecoArmazenagem { get; set; }

        [Required(ErrorMessage = "A impressora deve ser informada.")]
        public long IdImpressora { get; set; }

        [Required(ErrorMessage = "Tipo impressão deve ser informado.")]
        public EtiquetaEnderecoTipoImpressao TipoImpressao { get; set; }
    }
}