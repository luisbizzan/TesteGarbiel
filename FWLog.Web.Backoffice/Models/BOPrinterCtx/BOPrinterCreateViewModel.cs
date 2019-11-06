using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOPrinterCtx
{
    public class BOPrinterCreateViewModel
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Empresa")]
        [Required(ErrorMessage = "O campo Empresa é obrigatório.")]
        public long? CompanyId { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "O campo Tipo é obrigatório.")]
        public int? PrinterTypeId { get; set; }

        [Required]
        [Display(Name = "IP")]
        public string IP { get; set; }
    }
}