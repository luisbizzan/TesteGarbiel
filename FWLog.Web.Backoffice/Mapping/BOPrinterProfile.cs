using AutoMapper;
using ExtensionMethods;
using FWLog.Data.EnumsAndConsts;
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
                .ForMember(x => x.Ativa, op => op.MapFrom(x => ((NaoSimEnum)x.Ativa).GetDisplayName()))
                .ForMember(x => x.PrinterType, op => op.MapFrom(x => x.PrinterType.Name));
        }
    }
}