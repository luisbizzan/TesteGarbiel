using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum NotaFiscalTipoEnum
    {
        Compra = 1,
        DevolucaoCompra = 2,
    }
    public class NotaFiscalTipo
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public NotaFiscalTipoEnum IdNotaFiscalTipo { get; set; }
        [StringLength(50)]
        [Required]
        public string Descricao { get; set; }
    }
}
