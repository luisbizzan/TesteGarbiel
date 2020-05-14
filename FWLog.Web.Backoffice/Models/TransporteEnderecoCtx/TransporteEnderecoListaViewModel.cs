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
        [Display(Name = "Transportadora")]
        public string DadosTransportadora { get; set; }
        [Display(Name = "Endereço")]
        public string Codigo { get; set; }
        public long IdTransportadoraEndereco { get; set; }

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