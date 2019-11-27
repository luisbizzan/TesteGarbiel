using AutoMapper;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.Models.PontoArmazenagemCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class PontoArmazenagemProfile : Profile
    {
        public PontoArmazenagemProfile()
        {
            CreateMap<PontoArmazenagem, PontoArmazenagemEditarViewModel>()
                .ForMember(dest => dest.DescricaoNivelArmazenagem, opt => opt.MapFrom(src => src.NivelArmazenagem.Descricao));

            CreateMap<PontoArmazenagemEditarViewModel, PontoArmazenagem>();

            CreateMap<PontoArmazenagem, PontoArmazenagemDetalhesViewModel>()
                .ForMember(dest => dest.NivelArmazenagem, opt => opt.MapFrom(src => src.NivelArmazenagem.Descricao))
                .ForMember(dest => dest.PontoArmazenagem, opt => opt.MapFrom(src => src.Descricao))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Ativo ? "Ativo" : "Inativo"))
                .ForMember(dest => dest.TipoArmazenagem, opt => opt.MapFrom(src => src.TipoArmazenagem.Descricao))
                .ForMember(dest => dest.TipoMovimentacao, opt => opt.MapFrom(src => src.TipoMovimentacao.Descricao));
        }
    }
}