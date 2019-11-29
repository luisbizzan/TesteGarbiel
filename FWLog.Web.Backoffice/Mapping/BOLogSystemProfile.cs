using AutoMapper;
using DartDigital.Library.Web.Globalization;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Web.Backoffice.Models.BOLogSystemCtx;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Mapping
{
    public class BOLogSystemProfile : Profile
    {
        public BOLogSystemProfile()
        {
            CreateMap<BOLogSystemDetails, BOLogSystemDetailsViewModel>()
                .ForMember(x => x.ColumnChanges, op => op.MapFrom(x => x.ColumnChanges))
                .ForMember(x => x.RelatedLogs, op => op.MapFrom(x => x.RelatedLogs))
                .ForMember(x => x.ExecutionDate, op => op.MapFrom(x => DateTimeConvert.FromUtc(x.ExecutionDate).ToSessionTime()));

            CreateMap<IEnumerable<LogEntity>, SelectList>()
                .ConstructUsing(x => new SelectList(x.Select(y => new SelectListItem { Text = y.TranslatedName, Value = y.OriginalName }), "Value", "Text"));

            CreateMap<IEnumerable<ActionTypeNames>, SelectList>()
                .ConstructUsing(x => new SelectList(x.Select(y => new SelectListItem { Text = y.DisplayName, Value = y.Value }), "Value", "Text"));

            CreateMap<BOLogSystemColumnChanges, BOLogSystemColumnChangesViewModel>();

            CreateMap<BOLogSystemTableRow, BOLogSystemListItemViewModel>()
                .ForMember(x => x.UserName, op => op.MapFrom(x => x.UserName))
                .ForMember(x => x.ExecutionDate, op => op.MapFrom(x => DateTimeConvert.FromUtc(x.ExecutionDate).ToSessionTime()));

            CreateMap<BOLogSystemFilter, BOLogSystemFilterViewModel>()
                .ForMember(x => x.ExecutionDateStart, op => op.MapFrom(x => DateTimeConvert.FromSessionTime(x.ExecutionDateStart).ToUtc()))
                .ForMember(x => x.ExecutionDateEnd, op => op.MapFrom(x => DateTimeConvert.FromSessionTime(x.ExecutionDateEnd).ToUtc()));
        }
    }
}