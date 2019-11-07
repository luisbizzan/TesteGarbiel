using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class PrinterType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        #region Navigations

        public ICollection<Printer> Printers { get; set; }

        #endregion
    }
}
