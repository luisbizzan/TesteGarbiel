using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.TransporteEnderecoCtx
{
    public class TransporteEnderecoListaViewModel
    {
        public TransporteEnderecoListaItemViewModel Itens { get; set; }

        public TransporteEnderecoListaFiltroViewModel Filtros { get; set; }

        public TransporteEnderecoListaViewModel()
        {
            Itens = new TransporteEnderecoListaItemViewModel();
            Filtros = new TransporteEnderecoListaFiltroViewModel();
        }
    }

    public class TransporteEnderecoListaItemViewModel
    {
        [Display(Name = "CNPJ Transportadora")]
        public string CnpjTransportadora { get; set; }
        [Display(Name = "Razão Social")]
        public string RazaoSocialTransportadora { get; set; }
        [Display(Name = "Endereço")]
        public string CodigoEnderecoArmazenagem { get; set; }

    }

    public class TransporteEnderecoListaFiltroViewModel
    {
        [Display(Name = "Endereço de Armazenagem")]
        public long? IdEnderecoArmazenagem { get; set; }
        public string CodigoEnderecoArmazenagem { get; set; }
        [Display(Name = "Transportadora")]
        public long? IdTransportadora { get; set; }
        public string RazaoSocialTransportadora { get; set; }
    }
}