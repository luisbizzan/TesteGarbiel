using AutoMapper;
using FWLog.Web.Backoffice.Models.EmpresaCtx;
using FWLog.Data.Models;

namespace FWLog.Web.Backoffice.Mapping
{
    public class EmpresaProfile : Profile
    {
        public EmpresaProfile()
        {
            CreateMap<EmpresaConfig, EmpresaConfigEditarViewModel>()
             .ForMember(d => d.Empresa, opt => opt.MapFrom(s => s.Empresa))
             .ForMember(d => d.NomeFantasiaEmpresaGarantia, opt => opt.MapFrom(s => s.EmpresaGarantia.NomeFantasia))
             .ForMember(d => d.NomeFantasiaEmpresaMatriz, opt => opt.MapFrom(s => s.EmpresaMatriz.NomeFantasia))
             .ForMember(d => d.RazaoSocialTransportadora, opt => opt.MapFrom(s => s.Transportadora.RazaoSocial));

            CreateMap<Empresa, EmpresaDetalhesViewModel>()
                .ForMember(dest => dest.CNPJ, opt => opt.MapFrom(src => src.CNPJ.Substring(0, 2) + "." + src.CNPJ.Substring(2, 3) + "." + src.CNPJ.Substring(5, 3) + "/" + src.CNPJ.Substring(8, 4) + "-" + src.CNPJ.Substring(12, 2)));

            CreateMap<EmpresaConfigEditarViewModel, EmpresaConfig>()
                .ForMember(d => d.Empresa, opt => opt.Ignore())
                .ForMember(d => d.EmpresaGarantia, opt => opt.Ignore())
                .ForMember(d => d.EmpresaMatriz, opt => opt.Ignore())
                .ForMember(d => d.EmpresaTipo, opt => opt.Ignore());

        }
    }
}

