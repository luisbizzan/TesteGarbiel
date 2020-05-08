using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class FinalizarDespachoNFRequisicao
    {
        [Required(ErrorMessage = "A chave de acesso deve ser informada.")]
        public string ChaveAcesso { get; set; }

        [Required(ErrorMessage = "A transportadora deve ser informada.")]
        public long IdTransportadora { get; set; }
    }
}