namespace FWLog.Data.Models
{
    public class NotaFiscalItem
    {
        public int IdNotaFiscalItem { get; set; }
        public int IdNotaFiscal { get; set; }
        public int IdProduto { get; set; }
        public int IdUnidadeMedida { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }

        public virtual NotaFiscal NotaFiscal { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual UnidadeMedida UnidadeMedida { get; set; }
    }
}
