using System.ComponentModel.DataAnnotations;
using Res = Resources.PrinterTypeStrings;

namespace FWLog.Web.Backoffice.Models.PrinterTypeCtx
{
    public class PrinterTypeCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }
    }
}