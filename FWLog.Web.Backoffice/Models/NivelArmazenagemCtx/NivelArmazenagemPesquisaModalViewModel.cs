using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.NivelArmazenagemCtx
{
    public class NivelArmazenagemPesquisaModalViewModel
    {
        public NivelArmazenagemPesquisaModalViewModel()
        {
            Itens = new NivelArmazenagemPesquisaModalItemViewModel();
            Filtros = new NivelArmazenagemPesquisaModalFiltroViewModel();
        }

        public NivelArmazenagemPesquisaModalItemViewModel Itens { get; set; }
        public NivelArmazenagemPesquisaModalFiltroViewModel Filtros { get; set; }

        public SelectList Status { get; set; }
    }

    public class NivelArmazenagemPesquisaModalItemViewModel
    {
        [Display(Name = "Código")]
        public int IdNivelArmazenagem { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }

    public class NivelArmazenagemPesquisaModalFiltroViewModel
    {
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Status")]
        public bool? Status { get; set; }
    }
}