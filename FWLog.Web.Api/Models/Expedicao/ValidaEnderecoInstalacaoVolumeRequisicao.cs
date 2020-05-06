using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Api.Models.Expedicao
{
    public class ValidaEnderecoInstalacaoVolumeRequisicao : ValidaTransportadoraInstalacaoVolumeRequisicao
    {
        [Required]
        public long IdEnderecoArmazenagem { get; set; }
    }
}