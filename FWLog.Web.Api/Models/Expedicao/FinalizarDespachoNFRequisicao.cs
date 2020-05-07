using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class FinalizarDespachoNFRequisicao
    {
        [Required]
        public string ChaveAcesso { get; set; }

        [Required]
        public long IdTransportadora { get; set; }
    }
}