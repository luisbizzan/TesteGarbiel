using ExtensionMethods;
using FWLog.Data.EnumsAndConsts;
using System.ComponentModel.DataAnnotations;
using Res = Resources.BOPrinterStrings;

namespace FWLog.Web.Backoffice.Models.BOPrinterCtx
{
    public class BOPrinterDetailsViewModel
    {
        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }

        [Display(Name = "Tipo")]
        public string PrinterType { get; set; }

        [Display(Name = "Empresa")]
        public string Company { get; set; }

        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "Ativa")]
        public string Ativa
        {
            get
            {
                return _Status.GetDisplayName();
            }
        }

        public NaoSimEnum _Status { get; set; }
    }
}