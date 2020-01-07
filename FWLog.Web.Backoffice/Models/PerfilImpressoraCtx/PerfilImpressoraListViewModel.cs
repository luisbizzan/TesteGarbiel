using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.PerfilImpressoraCtx
{
    public class PerfilImpressoraListViewModel
    {
        public PerfilImpressoraListItemViewModel Itens { get; set; }

        public PerfilImpressoraFilterViewModel Filtro { get; set; }

        public SelectList Status { get; set; }

        public PerfilImpressoraListViewModel()
        {
            Itens = new PerfilImpressoraListItemViewModel();
            Filtro = new PerfilImpressoraFilterViewModel();
        }
    }

    public class PerfilImpressoraListItemViewModel
    {
        [Display(Name = "Código")]
        public long IdPerfilImpressora { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
    }

    public class PerfilImpressoraFilterViewModel
    {
        [Display(Name = "Empresa")]
        public long IdEmpresa { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Display(Name = "Status")]
        public int? Status { get; set; }
    }
}
