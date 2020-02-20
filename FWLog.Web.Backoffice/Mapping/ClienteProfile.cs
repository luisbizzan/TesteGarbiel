using AutoMapper;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Web.Backoffice.Models.ClienteCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClientePesquisaModalLinhaTabela, ClienteSearchModalItemViewModel>();
        }
    }
}