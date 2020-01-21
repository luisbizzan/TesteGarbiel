using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.EmpresaCtx
{
    public class EmpresaSearchModalViewModel
    {
        public EmpresaSearchModalItemViewModel EmptyItem { get; set; }
        public EmpresaSearchModalFilterViewModel Filter { get; set; }
        public string CampoSelecionado { get; set; }

        public EmpresaSearchModalViewModel(string campoSelecionado)
        {
            EmptyItem = new EmpresaSearchModalItemViewModel();
            Filter = new EmpresaSearchModalFilterViewModel(campoSelecionado);
            CampoSelecionado = campoSelecionado;
        }
    }

    public class EmpresaSearchModalItemViewModel
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

        [Display(Name = "Sigla")]
        public string Sigla { get; set; }
    }

    public class EmpresaSearchModalFilterViewModel
    {
        public EmpresaSearchModalFilterViewModel()
        {
        }

        public EmpresaSearchModalFilterViewModel(string campoSelecionado)
        {
            CampoSelecionado = campoSelecionado;
        }

        public string CampoSelecionado { get; set; }

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