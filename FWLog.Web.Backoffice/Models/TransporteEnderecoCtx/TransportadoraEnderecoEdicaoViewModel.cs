using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.TransporteEnderecoCtx
{
    public class TransportadoraEnderecoEdicaoViewModel
    {
        public long IdTransportadoraEndereco { get; set; }
        [Display(Name = "Nível Armazenagem")]
        [Required]
        public long IdNivelArmazenagem { get; set; }
        public string DescricaoNivelArmazenagem { get; set; }

        [Display(Name = "Ponto Armazenagem")]
        [Required]
        public long IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }

        [Display(Name = "Endereço Armazenagem")]
        [Required]
        public long IdEnderecoArmazenagem { get; set; }
        public string CodigoEnderecoArmazenagem { get; set; }

        [Display(Name = "Transportadora")]
        [Required]
        public long IdTransportadora { get; set; }
        public string RazaoSocialTransportadora { get; set; }
    }
}