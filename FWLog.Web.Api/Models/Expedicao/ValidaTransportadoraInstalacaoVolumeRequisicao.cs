using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class ValidaTransportadoraInstalacaoVolumeRequisicao
    {
        [Required]
        public long IdPedidoVendaVolume { get; set; }

        [Required]
        public long IdTransportadora { get; set; }
    }
}