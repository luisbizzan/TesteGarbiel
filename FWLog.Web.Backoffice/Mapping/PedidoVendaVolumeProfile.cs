using AutoMapper;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Web.Backoffice.Models.PedidoVendaVolumeCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class PedidoVendaVolumeProfile : Profile
    {
        public PedidoVendaVolumeProfile()
        {
            CreateMap<PedidoVendaVolumePesquisaModalLinhaTabela, PedidoVendaVolumeSearchModalItemViewModel>()
                .ForMember(dest => dest.NroVolume, opt => opt.MapFrom(src => src.NroVolume.ToString().PadLeft(3, '0')));
        }
    }
}