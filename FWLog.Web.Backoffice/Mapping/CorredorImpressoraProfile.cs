using AutoMapper;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.Models.CorredorImpressoraCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class CorredorImpressoraProfile : Profile
    {
        public CorredorImpressoraProfile()
        {
            CreateMap<CorredorImpressoraCadastroViewModel, GrupoCorredorArmazenagem>()
                    .ForMember(c => c.IdGrupoCorredorArmazenagem, opt => opt.Ignore())
                    .ForMember(c => c.IdImpressora, opt => opt.MapFrom(src => src.IdImpressora))
                    .ForMember(c => c.IdImpressoraPedidoFilial, opt => opt.MapFrom(src => src.IdImpressoraPedidoFilial))
                    .ForMember(c => c.Empresa, opt => opt.Ignore())
                    .ForMember(c => c.PontoArmazenagem, opt => opt.Ignore());

            CreateMap<GrupoCorredorArmazenagem, CorredorImpressoraDetalhesViewModel>()
            .ForMember(c => c.DescricaoPontoArmazenagem, opt => opt.MapFrom(src => src.PontoArmazenagem.Descricao))
            .ForMember(c => c.DescricaoImpressora, opt => opt.MapFrom(src => src.Impressora.Name))
            .ForMember(c => c.DescricaoImpressoraPedidoFilial, opt => opt.MapFrom(src => src.ImpressoraPedidoFilial.Name))
            .ForMember(c => c.Ativo, opt => opt.MapFrom(src => src.Ativo ? "Sim" : "Não"));

            CreateMap<GrupoCorredorArmazenagem, CorredorImpressoraEdicaoViewModel>()
                   .ForMember(c => c.ListaImpressora, opt => opt.Ignore())
                   .ForMember(c => c.IdImpressoraPedidoFilial, opt => opt.MapFrom(src => src.IdImpressoraPedidoFilial))
                   .ForMember(c => c.DescricaoPontoArmazenagem, opt => opt.MapFrom(src => src.PontoArmazenagem.Descricao));


            CreateMap<CorredorImpressoraEdicaoViewModel, GrupoCorredorArmazenagem>()
                   .ForMember(c => c.IdEmpresa, opt => opt.Ignore())
                   .ForMember(c => c.PontoArmazenagem, opt => opt.Ignore())
                   .ForMember(c => c.Empresa, opt => opt.Ignore())
                   .ForMember(c => c.IdImpressoraPedidoFilial, opt => opt.MapFrom(src => src.IdImpressoraPedidoFilial))
                   .ForMember(c => c.IdImpressora, opt => opt.MapFrom(src => src.IdImpressora));
        }
    }
}