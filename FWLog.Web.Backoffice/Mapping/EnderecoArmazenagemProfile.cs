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

            CreateMap<EnderecoArmazenagemEditarViewModel, EnderecoArmazenagem> ();

            CreateMap<EnderecoArmazenagem, EnderecoArmazenagemEditarViewModel>()
                .ForMember(dest => dest.DescricaoNivelArmazenagem, opt => opt.MapFrom(src => src.NivelArmazenagem.Descricao))
                .ForMember(dest => dest.DescricaoPontoArmazenagem, opt => opt.MapFrom(src => src.PontoArmazenagem.Descricao));

            CreateMap<EnderecoArmazenagem, EnderecoArmazenagemDetalhesViewModel>()
                .ForMember(dest => dest.NivelArmazenagem, opt => opt.MapFrom(src => src.NivelArmazenagem.Descricao))
                .ForMember(dest => dest.PontoArmazenagem, opt => opt.MapFrom(src => src.PontoArmazenagem.Descricao))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Ativo ? "Ativo" : "Inativo"))
                .ForMember(dest => dest.Fifo, opt => opt.MapFrom(src => src.IsFifo ? "Sim" : "Não"))
                .ForMember(dest => dest.PontoSeparacao, opt => opt.MapFrom(src => src.IsPontoSeparacao ? "Sim" : "Não"));
        }
    }
}