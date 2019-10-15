using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Web.Backoffice.Models.BOAccountCtx;
using System.Web.Security;

namespace FWLog.Web.Backoffice.Mapping
{
    public class BOAccountProfile : Profile
    {
        public BOAccountProfile()
        {
            CreateMap<ApplicationRole, GroupItemViewModel>()
                .ForMember(x => x.Name, op => op.MapFrom(x => x.Name));

            CreateMap<ApplicationUser, ApplicationUser>();

            CreateMap<ApplicationUser, BOAccountCreateViewModel>(); // TODO: Implementar IsApproved em Create, Edit e Details. Ou remover checkbox inativo.

            CreateMap<ApplicationUser, BOAccountEditViewModel>();

            CreateMap<ApplicationUser, BOAccountDetailsViewModel>();
        }
    }
}