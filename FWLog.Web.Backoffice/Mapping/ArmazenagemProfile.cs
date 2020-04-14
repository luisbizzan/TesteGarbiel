using AutoMapper;
using FWLog.Data.Models.FilterCtx;
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

            CreateMap<RelatorioTotalizacaoLocalizacaoFilterViewModel, RelatorioTotalizacaoLocalizacaoFiltro>()
                .ForMember(x => x.IdEmpresa, opt => opt.Ignore());

            CreateMap<RelatorioTotalizacaoLocalizacaoFiltro, RelatorioTotalizacaoLocalizacaoFilterViewModel>()
                .ForMember(x => x.DescricaoNivelArmazenagem, opt => opt.Ignore())
                .ForMember(x => x.DescricaoPontoArmazenagem, opt => opt.Ignore());
            CreateMap<DownloadRelatorioLogisticaCorredorViewModel, RelatorioLogisticaCorredorRequest>();
            CreateMap<ImprimirRelatorioLogisticaCorredorViewModel, ImprimirRelatorioLogisticaCorredorRequest>();
        }
    }
}