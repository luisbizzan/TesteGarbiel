namespace FWLog.Data.Models.DataTablesCtx
{
    public class GarantiaTableRow
    {
        public long IdGarantia { get; set; }
        public long? IdCliente { get; set; }
        public long IdTransportadora { get; set; }
        // TODO: Criar colunas do DataTable, deve conter as mesmas propriedades da classe GarantiaListItemViewModel
    }
}
