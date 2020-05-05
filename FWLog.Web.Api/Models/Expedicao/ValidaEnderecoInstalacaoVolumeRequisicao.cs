using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class ValidaEnderecoInstalacaoVolumeRequisicao
    {
        [Required]
        public long IdPedidoVendaVolume { get; set; }

        [Required]
        public long IdEnderecoArmazenagem { get; set; }
    }
}