using System.ComponentModel.DataAnnotations;
using Res = Resources.PrinterTypeStrings;

namespace FWLog.Web.Backoffice.Models.PrinterTypeCtx
{
    public class PrinterTypeDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }

    }
}