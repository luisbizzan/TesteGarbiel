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

            CreateMap<GarantiaSolicitacaoFilterVM, GarantiaFilter>();

            CreateMap<GarSolicitacaoItem, GarantiaSolicitacaoItemListVM>();

            CreateMap<GarSolicitacao, GarantiaRemessaListVM>();

            CreateMap<GarConferenciaItem, GarantiaConferenciaItem>();

            CreateMap<GarConferenciaHist, GarantiaConferenciaItem>();

            CreateMap<GarSolicitacaoItemLaudo, GarantiaLaudo>();
        }
    }
}