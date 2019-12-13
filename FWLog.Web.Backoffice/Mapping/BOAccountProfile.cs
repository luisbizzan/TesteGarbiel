using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Web.Backoffice.Models.BOAccountCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class BOAccountProfile : Profile
    {
        public BOAccountProfile()
        {
            CreateMap<ApplicationRole, GroupItemViewModel>()
                .ForMember(x => x.Name, op => op.MapFrom(x => x.Name));

            CreateMap<ApplicationUser, ApplicationUser>();
            CreateMap<ApplicationUser, BOAccountCreateViewModel>();
            CreateMap<ApplicationUser, BOAccountEditViewModel>();
            CreateMap<ApplicationUser, BOAccountDetailsViewModel>();
            CreateMap<UsuarioListaLinhaTabela, BOAccountListItemViewModel>();
        }
    }
}