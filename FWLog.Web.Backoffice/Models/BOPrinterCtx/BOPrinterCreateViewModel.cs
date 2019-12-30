using System.ComponentModel.DataAnnotations;
using Res = Resources.BOPrinterStrings;

namespace FWLog.Web.Backoffice.Models.BOPrinterCtx
{
    public class BOPrinterCreateViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }
        [Required(ErrorMessage = "O campo Tipo é obrigatório.")]
        [Display(Name = nameof(Res.PrinterTypeLabel), ResourceType = typeof(Res))]
        public int? PrinterTypeId { get; set; }
        [Required(ErrorMessage = "O campo Endereço IP é obrigatório.")]
        [Display(Name = nameof(Res.IPLabel), ResourceType = typeof(Res))]
        public string IP { get; set; }
        [Display(Name = "Ativa?")]
        [Required(ErrorMessage = "O campo Ativa é obrigatório.")]
        public bool Ativa { get; set; }
    }
}