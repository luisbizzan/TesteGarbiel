using AutoMapper;
using ExtensionMethods.String;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Models.NivelArmazenagemCtx;
using static FWLog.AspNet.Identity.Permissions;

namespace FWLog.Web.Backoffice.Mapping
{
    public class NivelArmazenagemProfile : Profile
    {
        public NivelArmazenagemProfile()
        {
            CreateMap<NivelArmazenagemTableRow, NivelArmazenagemListItemViewModel>()
                .ForMember(x => x.Status, op => op.MapFrom(x => x.Ativo.BooleanResource()));

            CreateMap<NivelArmazenagem, NivelArmazenagemCreateViewModel>().ReverseMap();

            CreateMap<NivelArmazenagem, NivelArmazenagemDetailsViewModel>();

            CreateMap<NivelArmazenagemFilterViewModel, NivelArmazenagemFilter>();
        }
    }
}