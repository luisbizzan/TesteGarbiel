using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.NivelArmazenagemCtx
{
    public class NivelArmazenagemListViewModel
    {
        public NivelArmazenagemListItemViewModel EmptyItem { get; set; }

        public NivelArmazenagemFilterViewModel Filter { get; set; }

        public NivelArmazenagemListViewModel()
        {
            EmptyItem = new NivelArmazenagemListItemViewModel();
            Filter = new NivelArmazenagemFilterViewModel();
        }
    }

    public class NivelArmazenagemListItemViewModel
    {
        [Display(Name = "Cód")]
        public long IdNivelArmazenagem { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        public string Status { get; set; }
    }

    public class NivelArmazenagemFilterViewModel
    {
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        public bool? Status { get; set; }
    }
}