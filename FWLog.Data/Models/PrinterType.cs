using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class PrinterType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        #region Navigations

        public ICollection<Printer> Printers { get; set; }

        #endregion
    }
}
