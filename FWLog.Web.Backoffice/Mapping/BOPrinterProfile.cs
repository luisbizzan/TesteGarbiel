using AutoMapper;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.Models.BOPrinterCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class BOPrinterProfile : Profile
    {
        public BOPrinterProfile()
        {
            CreateMap<Printer, BOPrinterListItemViewModel>()
                .ForMember(x => x.Company, op => op.MapFrom(x => x.Company.CompanyName))
                .ForMember(x => x.PrinterType, op => op.MapFrom(x => x.PrinterType.Name));
        }
    }
}