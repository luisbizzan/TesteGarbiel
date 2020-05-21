using AutoMapper;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Models.ExpedicaoCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class RelatorioVolumesInstaladosTransportadoraFiltroProfile : Profile
    {
        public RelatorioVolumesInstaladosTransportadoraFiltroProfile()
        {
            CreateMap<RelatorioVolumesInstaladosTransportadoraFiltro, RelatorioVolumesInstaladosTransportadoraFilterViewModel>()
                .ForMember(x => x.NomeTransportadora, opt => opt.Ignore())
                .ForMember(x => x.TransportadoraEndereco, opt => opt.Ignore())
                .ForMember(x => x.NumeroPedidoVenda, opt => opt.Ignore());

            CreateMap<RelatorioVolumesInstaladosTransportadoraFilterViewModel, RelatorioVolumesInstaladosTransportadoraFiltro>()
              .ForMember(x => x.IdEmpresa, opt => opt.Ignore());
        }
    }
}