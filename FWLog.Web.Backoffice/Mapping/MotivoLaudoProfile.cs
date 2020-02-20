using AutoMapper;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Web.Backoffice.Models.MotivoLaudoCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class MotivoLaudoProfile : Profile
    {
        public MotivoLaudoProfile()
        {
            CreateMap<MotivoLaudoLinhaTabela, MotivoLaudoListItemViewModel>();
        }
    }
}