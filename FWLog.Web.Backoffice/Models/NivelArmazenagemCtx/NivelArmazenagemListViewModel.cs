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
        public long IdNivelArmazenagem { get; set; }

        // TODO: Criar colunas do DataTable, deve conter as mesmas propriedades da classe NivelArmazenagemTableRow
    }

    public class NivelArmazenagemFilterViewModel
    {
        // TODO: Criar propriedades de filtro, deve conter os mesmos campos da classe NivelArmazenagemFilter
    }
}