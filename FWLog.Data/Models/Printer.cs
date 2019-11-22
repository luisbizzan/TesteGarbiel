using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Printer
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int? PrinterTypeId { get; set; }

        [Required]
        public long? CompanyId { get; set; }

        [Required]
        [StringLength(50)]
        public string IP { get; set; }

        [Required]
        public int? Ativa { get; set; }

        #region ForeignKey

        [ForeignKey("CompanyId")]
        public Empresa Empresa { get; set; }

        [ForeignKey(nameof(PrinterTypeId))]
        public PrinterType PrinterType { get; set; }

        #endregion
    }
}
