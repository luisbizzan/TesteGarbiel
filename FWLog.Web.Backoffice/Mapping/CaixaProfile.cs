using AutoMapper;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Web.Backoffice.Models.CaixaCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class CaixaProfile : Profile
    {
        public CaixaProfile()
        {
            CreateMap<CaixaCadastroViewModel, Caixa>()
                    .ForMember(c => c.IdCaixa, opt => opt.Ignore())
                    .ForMember(c => c.IdEmpresa, opt => opt.Ignore())
                    .ForMember(c => c.Cubagem, opt => opt.Ignore())
                    .ForMember(c => c.Empresa, opt => opt.Ignore())
                    .ForMember(c => c.CaixaTipo, opt => opt.Ignore());

            CreateMap<Caixa, CaixaDetalhesViewModel>()
             .ForMember(caixa => caixa.CaixaTipoDescricao, opt => opt.MapFrom(src => src.CaixaTipo.Descricao))
             .ForMember(caixa => caixa.Ativo, opt => opt.MapFrom(src => src.Ativo ? "Sim" : "Não"))
             .ForMember(caixa => caixa.PesoMaximo, opt => opt.MapFrom(src => src.PesoMaximo.ToString("N2")))
             .ForMember(caixa => caixa.PesoCaixa, opt => opt.MapFrom(src => src.PesoCaixa.ToString("N2")))
             .ForMember(caixa => caixa.Sobra, opt => opt.MapFrom(src => src.Sobra.ToString("N2")));

            CreateMap<Caixa, CaixaEdicaoViewModel>()
                   .ForMember(caixa => caixa.ListaCaixaTipo, opt => opt.Ignore())
                   .ForMember(caixa => caixa.PesoMaximo, opt => opt.MapFrom(src => src.PesoMaximo.ToString("N2")))
                   .ForMember(caixa => caixa.PesoCaixa, opt => opt.MapFrom(src => src.PesoCaixa.ToString("N2")))
                   .ForMember(caixa => caixa.Sobra, opt => opt.MapFrom(src => src.Sobra.ToString("N2")));

            CreateMap<CaixaEdicaoViewModel, Caixa>()
                   .ForMember(c => c.IdEmpresa, opt => opt.Ignore())
                   .ForMember(c => c.Cubagem, opt => opt.Ignore())
                   .ForMember(c => c.Empresa, opt => opt.Ignore())
                   .ForMember(c => c.CaixaTipo, opt => opt.Ignore());
        }
    }
}