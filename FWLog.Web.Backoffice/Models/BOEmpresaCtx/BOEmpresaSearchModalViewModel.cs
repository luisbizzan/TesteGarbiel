using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOEmpresaCtx
{
    public class BOEmpresaSearchModalViewModel
    {
        public BOEmpresaSearchModalItemViewModel EmptyItem { get; set; }
        public BOEmpresaSearchModalFilterViewModel Filter { get; set; }
        public string CampoSelecionado { get; set; }

        public BOEmpresaSearchModalViewModel(string campoSelecionado)
        {
            EmptyItem = new BOEmpresaSearchModalItemViewModel();
            Filter = new BOEmpresaSearchModalFilterViewModel(campoSelecionado);
            CampoSelecionado = campoSelecionado;
        }
    }

    public class BOEmpresaSearchModalItemViewModel
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Código")]
        public long CodigoIntegracao { get; set; }

        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Display(Name = "Sigla")]
        public string Sigla { get; set; }
    }

    public class BOEmpresaSearchModalFilterViewModel
    {
        public BOEmpresaSearchModalFilterViewModel()
        {

        }

        public BOEmpresaSearchModalFilterViewModel(string campoSelecionado)
        {
            CampoSelecionado = campoSelecionado;
        }

        public string CampoSelecionado { get; set; }

        [Display(Name = "Código")]
        public long? CodigoIntegracao { get; set; }

        [Display(Name = "Razão Social")]
        [StringLength(40)]
        public string RazaoSocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        [StringLength(40)]
        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ")]
        [StringLength(14)]
        public string CNPJ { get; set; }

        [Display(Name = "Sigla")]
        [StringLength(3)]
        public string Sigla { get; set; }
    }
}