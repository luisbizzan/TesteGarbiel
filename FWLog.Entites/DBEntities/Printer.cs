using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Entites.DBEntities
{
    public class Printer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int PrinterTypeId { get; set; }

        [Required]
        public long CompanyId { get; set; }

        [Required]
        [StringLength(50)]
        public string IP { get; set; }

        #region ForeignKey

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        [ForeignKey(nameof(PrinterTypeId))]
        public PrinterType PrinterType { get; set; }

        #endregion
    }
}
