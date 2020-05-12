using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.TransportadoraCtx
{
    public class TransportadoraSearchModalViewModel
    {
        public TransportadoraSearchModalItemViewModel EmptyItem { get; set; }
        public TransportadoraSearchModalFillterViewModel Filter { get; set; }

        public TransportadoraSearchModalViewModel()
        {
            EmptyItem = new TransportadoraSearchModalItemViewModel();
            Filter = new TransportadoraSearchModalFillterViewModel();
        }
    }

    public class TransportadoraSearchModalItemViewModel
    {
        public long CodigoIntegracao { get; set; }

        [Display(Name = "Código")]
        public long IdTransportadora { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Display(Name = "Ativo")]
        public string Status { get; set; }
    }

    public class TransportadoraSearchModalFillterViewModel
    {
        [Display(Name = "Código")]
        public long? IdTransportadora { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        public bool? Ativo { get; set; }
    }
}