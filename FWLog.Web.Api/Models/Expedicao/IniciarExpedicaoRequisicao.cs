using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class IniciarExpedicaoRequisicao
    {
        [Required]
        public long IdPedidoVenda { get; set; }

        [Required]
        public long IdPedidoVendaVolume { get; set; }
    }
}