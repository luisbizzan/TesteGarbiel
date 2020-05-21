using AutoMapper;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.Models.GarantiaCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class GarantiaProfile : Profile
    {
        public GarantiaProfile()
        {
            CreateMap<GarSolicitacao, GarantiaSolicitacaoListVM>();

            CreateMap<GarantiaSolicitacaoFilterVM, GarantiaSolicitacaoFilter>();

            CreateMap<GarSolicitacaoItem, GarantiaSolicitacaoItemListVM>();

            CreateMap<GarConferenciaItem, GarantiaSolicitacaoItemListVM>();

            CreateMap<GarRemessa, GarantiaRemessaListVM>();

            CreateMap<GarantiaRemessaFilterVM, GarantiaRemessaFilter>();

            CreateMap<GarConferenciaItem, GarantiaConferenciaItem>();

            CreateMap<GarConferenciaHist, GarantiaConferenciaItem>();

            CreateMap<GarSolicitacaoItemLaudo, GarantiaLaudo>();
        }
    }
}