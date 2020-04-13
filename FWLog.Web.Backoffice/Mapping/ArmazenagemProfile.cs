using AutoMapper;
using FWLog.Services.Model.Relatorios;
using FWLog.Web.Backoffice.Models.ArmazenagemCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class ArmazenagemProfile : Profile
    {
        public ArmazenagemProfile()
        {
            CreateMap<DownloadRelatorioTotalPorAlaViewModel, RelatorioTotalPorAlaRequest>();
            CreateMap<ImprimirRelatorioTotalPorAlaViewModel, ImprimirRelatorioTotalPorAlaRequest>();
            CreateMap<DownloadRelatorioPosicaoInventarioViewModel, RelatorioPosicaoInventarioRequest>();
            CreateMap<ImprimirRelatorioPosicaoInventarioViewModel, ImprimirRelatorioPosicaoInventarioRequest>();
            CreateMap<DownloadRelatorioLogisticaCorredorViewModel, RelatorioLogisticaCorredorRequest>();
            CreateMap<ImprimirRelatorioLogisticaCorredorViewModel, ImprimirRelatorioLogisticaCorredorRequest>();
        }
    }
}