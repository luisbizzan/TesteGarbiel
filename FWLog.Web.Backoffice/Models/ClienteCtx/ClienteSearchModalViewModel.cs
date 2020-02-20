using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ClienteCtx
{
    public class ClienteSearchModalViewModel
    {
        public ClienteSearchModalItemViewModel EmptyItem { get; set; }
        public ClienteSearchModalFillterViewModel Filter { get; set; }

        public ClienteSearchModalViewModel()
        {
            EmptyItem = new ClienteSearchModalItemViewModel();
            Filter = new ClienteSearchModalFillterViewModel();
        }
    }

    public class ClienteSearchModalItemViewModel
    {
        public long CodigoIntegracao { get; set; }

        [Display(Name = "Código")]
        public long IdCliente { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ/CPF")]
        public string CNPJCPF { get; set; }

        [Display(Name = "Classificação")]
        public string Classificacao { get; set; }

        [Display(Name = "Ativo")]
        public string Status { get; set; }
    }

    public class ClienteSearchModalFillterViewModel
    {
        [Display(Name = "Código")]
        public long? IdCliente { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ/CPF")]
        public string CNPJCPF { get; set; }
    }
}