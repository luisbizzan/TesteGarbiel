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
             .ForMember(d => d.RazaoSocialEmpresaGarantia, opt => opt.MapFrom(s => s.EmpresaGarantia.RazaoSocial))
             .ForMember(d => d.RazaoSocialEmpresaMatriz, opt => opt.MapFrom(s => s.EmpresaMatriz.RazaoSocial));

            CreateMap<Empresa, EmpresaDetalhesViewModel>();

            CreateMap<EmpresaConfigEditarViewModel, EmpresaConfig>()
                .ForMember(d => d.Empresa, opt => opt.Ignore())
                .ForMember(d => d.EmpresaGarantia, opt => opt.Ignore())
                .ForMember(d => d.EmpresaMatriz, opt => opt.Ignore())
                .ForMember(d => d.EmpresaTipo, opt => opt.Ignore());

        }
    }
}

