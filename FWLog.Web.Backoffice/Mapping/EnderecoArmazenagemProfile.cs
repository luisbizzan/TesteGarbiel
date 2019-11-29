using AutoMapper;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Models.EnderecoArmazenagemCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class EnderecoArmazenagemProfile : Profile
    {
        public EnderecoArmazenagemProfile()
        {
            CreateMap<EnderecoArmazenagemListaFilterViewModel, EnderecoArmazenagemListaFiltro>();
            CreateMap<EnderecoArmazenagemListaLinhaTabela, EnderecoArmazenagemListaItemViewModel>();
            CreateMap<EnderecoArmazenagemCadastroViewModel, EnderecoArmazenagem>();
        }
    }
}