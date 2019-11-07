namespace FWLog.Web.Backoffice.Models.PrinterTypeCtx
{
    public class PrinterTypeListViewModel
    {
        public PrinterTypeListItemViewModel EmptyItem { get; set; }

        public PrinterTypeFilterViewModel Filter { get; set; }

        public PrinterTypeListViewModel()
        {
            EmptyItem = new PrinterTypeListItemViewModel();
            Filter = new PrinterTypeFilterViewModel();
        }
    }

    public class PrinterTypeListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // TODO: Criar colunas do DataTable, deve conter as mesmas propriedades da classe PrinterTypeTableRow
    }

    public class PrinterTypeFilterViewModel
    {
        public string Name { get; set; }

        // TODO: Criar propriedades de filtro, deve conter os mesmos campos da classe PrinterTypeFilter
    }
}