using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.AspNet.Identity.Building;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Models.BOGroupCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace FWLog.Web.Backoffice.Mapping
{
    public class BOGroupProfile : Profile
    {
        public BOGroupProfile()
        {
            CreateMap<ApplicationRole, BOGroupCreateViewModel>().ReverseMap();

            CreateMap<ApplicationRole, ApplicationRole>();

            CreateMap<ApplicationRole, BOGroupDetailsViewModel>();

            CreateMap<BOGroupTableRow, BOGroupListItemViewModel>();

            CreateMap<BOGroupFilterViewModel, BOGroupFilter>();

            CreateMap<PermissionGroupBuildItem, PermissionGroupViewModel>()
                .ForMember(x => x.DisplayName, op => op.MapFrom(x => x.GetDisplayName()))
                .ForMember(x => x.Permissions, op => op.MapFrom(x => x.Permissions));

            CreateMap<PermissionBuildItem, PermissionItemViewModel>()
                .ForMember(x => x.DisplayName, op => op.MapFrom(x => x.GetDisplayName()))
                .ForMember(x => x.Name, op => op.MapFrom(x => x.Name));
        }
    }
}