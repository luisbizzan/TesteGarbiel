using AutoMapper;
using ExtensionMethods.String;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.Models.TransporteEnderecoCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class TransportadoraEnderecoProfile : Profile
    {
        public TransportadoraEnderecoProfile()
        {
            CreateMap<TransportadoraEndereco, TransportadoraEnderecoDetalhesViewModel>()
            .ForMember(c => c.RazaoSocialTransportadora, opt => opt.MapFrom(src => src.Transportadora.RazaoSocial))
            .ForMember(c => c.CnpjTransportadora, opt => opt.MapFrom(src => src.Transportadora.CNPJ.CnpjOuCpf()))
            .ForMember(c => c.Codigo, opt => opt.MapFrom(src => src.EnderecoArmazenagem.Codigo));

            CreateMap<TransportadoraEnderecoCadastroViewModel, TransportadoraEndereco>()
                   .ForMember(c => c.EnderecoArmazenagem, opt => opt.Ignore());

            CreateMap<TransportadoraEndereco, TransportadoraEnderecoEdicaoViewModel>()
                   .ForMember(c => c.DescricaoNivelArmazenagem, opt => opt.MapFrom(src => src.EnderecoArmazenagem.NivelArmazenagem.Descricao))
                   .ForMember(c => c.DescricaoPontoArmazenagem, opt => opt.MapFrom(src => src.EnderecoArmazenagem.PontoArmazenagem.Descricao))
                   .ForMember(c => c.CodigoEnderecoArmazenagem, opt => opt.MapFrom(src => src.EnderecoArmazenagem.Codigo))
                   .ForMember(c => c.RazaoSocialTransportadora, opt => opt.MapFrom(src => src.Transportadora.RazaoSocial))
                   .ForMember(c => c.IdNivelArmazenagem, opt => opt.MapFrom(src => src.EnderecoArmazenagem.IdNivelArmazenagem))
                   .ForMember(c => c.IdPontoArmazenagem, opt => opt.MapFrom(src => src.EnderecoArmazenagem.IdPontoArmazenagem));

            CreateMap<TransportadoraEnderecoEdicaoViewModel, TransportadoraEndereco>()
                  .ForMember(c => c.EnderecoArmazenagem, opt => opt.Ignore());
        }
    }
}