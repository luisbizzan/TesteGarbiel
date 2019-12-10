﻿using System.ComponentModel.DataAnnotations;
using Res = Resources.BOPrinterStrings;

namespace FWLog.Web.Backoffice.Models.BOPrinterCtx
{
    public class BOPrinterListViewModel
    {
        public BOPrinterListItemViewModel EmptyItem { get; set; }

        public BOPrinterFilterViewModel Filter { get; set; }

        public BOPrinterListViewModel()
        {
            EmptyItem = new BOPrinterListItemViewModel();
            Filter = new BOPrinterFilterViewModel();
        }
    }

    public class BOPrinterListItemViewModel
    {
        public long Id { get; set; }

        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }

        [Display(Name = nameof(Res.PrinterTypeLabel), ResourceType = typeof(Res))]
        public string PrinterType { get; set; }

        [Display(Name = nameof(Res.CompanyLabel), ResourceType = typeof(Res))]
        public string Empresa { get; set; }

        public string Status { get; set; }
    }

    public class BOPrinterFilterViewModel
    {
        [Display(Name = nameof(Res.NameLabel), ResourceType = typeof(Res))]
        public string Name { get; set; }

        [Display(Name = nameof(Res.CompanyLabel), ResourceType = typeof(Res))]
        public long? IdEmpresa { get; set; }

        [Display(Name = nameof(Res.PrinterTypeLabel), ResourceType = typeof(Res))]
        public int? PrinterTypeId { get; set; }

        public bool? Status { get; set; }
    }
}