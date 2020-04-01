using AutoMapper;
using FWLog.Data.Models;
using FWLog.Web.Api.Models.Produto;

namespace FWLog.Web.Api.Mapping
{
    public class ProdutoModelResponseProfile : Profile
    {
        public ProdutoModelResponseProfile()
        {
            CreateMap<Produto, PesquisarPorCodigoBarrasReferenciaResposta>()
                .ForMember(x => x.CodigoEnderecoPicking, opt => opt.Ignore());
        }
    }
}