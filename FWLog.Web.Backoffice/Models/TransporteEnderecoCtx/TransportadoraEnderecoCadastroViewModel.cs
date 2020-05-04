using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.TransporteEnderecoCtx
{
    public class TransportadoraEnderecoCadastroViewModel
    {
        [Display(Name = "Nível Armazenagem")]
        public long IdNivelArmazenagem { get; set; }
        public string DescricaoNivelArmazenagem { get; set; }

        [Display(Name = "Ponto Armazenagem")]
        public long IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }

        [Display(Name = "Endereço Armazenagem")]
        public long IdEnderecoArmazenagem { get; set; }
        public string CodigoEnderecoArmazenagem { get; set; }

        [Display(Name = "Transportadora")]
        public long IdTransportadora { get; set; }
        public string RazaoSocialTransportadora { get; set; }
    }
}