using AutoMapper;
using DartDigital.Library.Web.Globalization;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Models.ApplicationLogCtx;
using System;

namespace FWLog.Web.Backoffice.Mapping
{
    public class ApplicationLogProfile : Profile
    {
        public ApplicationLogProfile()
        {
            CreateMap<ApplicationLog, ApplicationLogDetailsViewModel>()
                .ForMember(x => x.ApplicationName, op => op.MapFrom(x => MapApplicationEnum(Convert.ToInt32(x.IdApplication))))
                .ForMember(x => x.Created, op => op.MapFrom(x => DateTimeConvert.FromUtc(x.Created).ToSessionTime()));

            CreateMap<ApplicationLogTableRow, ApplicationLogListItemViewModel>()
                .ForMember(x => x.Created, op => op.MapFrom(x => DateTimeConvert.FromUtc(x.Created).ToSessionTime()))
                .AfterMap((entity, model) =>
                {
                    if (model.Message != null && model.Message.Length > 75)
                    {
                        model.Message = string.Concat(model.Message.Substring(0, 75), "...");
                    }
                });

            CreateMap<ApplicationLogFilterViewModel, ApplicationLogFilter>()
                .ForMember(x => x.CreatedStart, op => op.MapFrom(x => DateTimeConvert.FromSessionTime(x.CreatedStart).ToUtc()))
                .ForMember(x => x.CreatedEnd, op => op.MapFrom(x => DateTimeConvert.FromSessionTime(x.CreatedEnd).ToUtc()));
        }

        public string MapApplicationEnum(int idApplication)
        {
            return ((ApplicationEnum)idApplication).GetValueString();
        }
    }
}