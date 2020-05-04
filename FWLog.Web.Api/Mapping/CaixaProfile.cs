using AutoMapper;
using FWLog.Data.Models;
using FWLog.Web.Api.Models.Caixa;

namespace FWLog.Web.Api.Mapping
{
    public class CaixaProfile : Profile
    {
        public CaixaProfile()
        {
            CreateMap<Caixa, CaixaResposta>()
                .ForMember(cr => cr.IdCaixaTipo, m => m.MapFrom(c => (int)c.CaixaTipo.IdCaixaTipo))
                .ForMember(cr => cr.DescricaoCaixaTipo, m => m.MapFrom(c => c.CaixaTipo.Descricao));
        }
    }
}