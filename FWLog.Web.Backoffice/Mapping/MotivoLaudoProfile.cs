using AutoMapper;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Web.Backoffice.Models.GarantiaCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class MotivoLaudoProfile : Profile
    {
        public MotivoLaudoProfile()
        {
            CreateMap<MotivoLaudoTableRow, MotivoLaudoListItemViewModel>();
        }
    }
}