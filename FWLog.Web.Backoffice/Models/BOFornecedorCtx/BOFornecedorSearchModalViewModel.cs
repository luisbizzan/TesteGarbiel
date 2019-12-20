using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOFornecedorCtx
{
    public class BOFornecedorSearchModalViewModel
    {
        public BOFornecedorSearchModalItemViewModel EmptyItem { get; set; }
        public BOFornecedorSearchModalFilterViewModel Filter { get; set; }

        public BOFornecedorSearchModalViewModel()
        {
            EmptyItem = new BOFornecedorSearchModalItemViewModel();
            Filter = new BOFornecedorSearchModalFilterViewModel();
        }
    }

    public class BOFornecedorSearchModalItemViewModel
    {
        public long IdFornecedor { get; set; }

        [Display(Name = "Código")]
        public long CodigoIntegracao { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }
    }

    public class BOFornecedorSearchModalFilterViewModel
    {
        [Display(Name = "Código")]
        public long? CodigoIntegracao { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Display(Name = "NomeFantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }
    }
}