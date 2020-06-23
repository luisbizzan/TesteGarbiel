using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class RomaneioReimprimirRequisicao
    {
        [Required(ErrorMessage = "O romaneio deve ser informado.")]
        public long IdRomaneio { get; set; }

        [Required(ErrorMessage = "A impressora deve ser informada.")]
        public long IdImpressora { get; set; }

        public bool ImprimeSegundaVia { get; set; }
    }
}