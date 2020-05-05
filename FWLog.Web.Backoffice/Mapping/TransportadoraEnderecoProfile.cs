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
        }

       
    }
}