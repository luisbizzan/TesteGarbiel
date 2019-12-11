using AutoMapper;
using ExtensionMethods.String;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.Models.BOPrinterCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class BOPrinterProfile : Profile
    {
        public BOPrinterProfile()
        {
            CreateMap<Printer, BOPrinterListItemViewModel>()
                .ForMember(x => x.Empresa, op => op.MapFrom(x => x.Empresa.RazaoSocial))
                .ForMember(x => x.Status, op => op.MapFrom(x => x.Ativa.BooleanResource()))
                .ForMember(x => x.PrinterType, op => op.MapFrom(x => x.PrinterType.Name));
        }
    }
}