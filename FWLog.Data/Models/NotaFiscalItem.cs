using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class NotaFiscalItem
    {
        [Key]
        public long IdNotaFiscalItem { get; set; }
        public long IdNotaFiscal { get; set; }
        public long IdProduto { get; set; }
        public long IdUnidadeMedida { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public long CodigoNotaFiscal { get; set; }

        [ForeignKey(nameof(IdNotaFiscal))]
        public NotaFiscal NotaFiscal { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public Produto Produto { get; set; }

        [ForeignKey(nameof(IdUnidadeMedida))]
        public UnidadeMedida UnidadeMedida { get; set; }
    }
}
