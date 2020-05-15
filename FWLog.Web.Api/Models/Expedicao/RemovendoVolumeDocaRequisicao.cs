using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class RemovendoVolumeDocaRequisicao
    {
        [Required] 
        public long IdPedidoVendaVolume { get; set; }
        [Required]
        public long IdTransprotadora { get; set; }
    }
}