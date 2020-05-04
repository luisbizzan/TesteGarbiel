using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.TransporteEnderecoCtx
{
    public class TransportadoraEnderecoDetalhesViewModel
    {
        public long IdEmpresa { get; set; }
        public long IdTransportadoraEndereco { get; set; }
        [Display(Name = "Razão Social")]
        public string RazaoSocialTransportadora { get; set; }

        [Display(Name = "CNPJ")]
        public string CnpjTransportadora { get; set; }

        [Display(Name = "Endereço")]
        public string Codigo { get; set; }
    }
}