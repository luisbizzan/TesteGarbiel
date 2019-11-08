using System.ComponentModel.DataAnnotations;
using Res = Resources.PrinterTypeStrings;

namespace FWLog.Web.Backoffice.Models.PrinterTypeCtx
{
    public class PrinterTypeListViewModel
    {
        public PrinterTypeListItemViewModel EmptyItem { get; set; }

        public PrinterTypeFilterViewModel Filter { get; set; }

        public PrinterTypeListViewModel()
        {
            EmptyItem = new PrinterTypeListItemViewModel();
            Filter = new PrinterTypeFilterViewModel();
        }
    }

    public class PrinterTypeListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }
    }

    public class PrinterTypeFilterViewModel
    {
        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }
    }
}