using AutoMapper;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Models.PrinterTypeCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class PrinterTypeProfile : Profile
    {
        public PrinterTypeProfile()
        {
            CreateMap<PrinterTypeTableRow, PrinterTypeListItemViewModel>();

            CreateMap<PrinterType, PrinterTypeCreateViewModel>().ReverseMap();

            CreateMap<PrinterType, PrinterTypeDetailsViewModel>();

            CreateMap<PrinterTypeFilterViewModel, PrinterTypeFilter>();
        }
    }
}