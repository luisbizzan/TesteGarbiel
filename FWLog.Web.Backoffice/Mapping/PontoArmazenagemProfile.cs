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
        }
    }
}