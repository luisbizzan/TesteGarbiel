namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaListViewModel
    {
        public GarantiaListItemViewModel EmptyItem { get; set; }

        public GarantiaFilterViewModel Filter { get; set; }

        public GarantiaListViewModel()
        {
            EmptyItem = new GarantiaListItemViewModel();
            Filter = new GarantiaFilterViewModel();
        }
    }

    public class GarantiaListItemViewModel
    {
        public long IdGarantia { get; set; }
        public long IdCliente { get; set; }
        public long IdTransportadora { get; set; }

        // TODO: Criar colunas do DataTable, deve conter as mesmas propriedades da classe GarantiaTableRow
    }

    public class GarantiaFilterViewModel
    {
        public long IdGarantia { get; set; }
        public long IdCliente { get; set; }
        public long IdTransportadora { get; set; }
        // TODO: Criar propriedades de filtro, deve conter os mesmos campos da classe GarantiaFilter
    }
}