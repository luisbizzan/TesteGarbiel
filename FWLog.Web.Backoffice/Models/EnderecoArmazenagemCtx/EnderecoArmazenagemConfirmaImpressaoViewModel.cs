using FWLog.Services.Model.Etiquetas;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx
{
    public class EnderecoArmazenagemConfirmaImpressaoViewModel
    {
        [Display(Name = "Código")]
        public long IdEnderecoArmazenagem { get; set; }

        [Display(Name = "Endereco Armazenagem")]
        public string Codigo { get; set; }

        [Display(Name = "Tipo Impressão")]
        public EtiquetaEnderecoTipoImpressao TipoImpressao { get; set; }
    }
}