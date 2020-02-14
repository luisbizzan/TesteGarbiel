using AutoMapper;
using FWLog.Data.Models;
using FWLog.Web.Api.Models.Usuario;

namespace FWLog.Web.Api.Mapping
{
    public class LoginModelResponseProfile : Profile
    {
        public LoginModelResponseProfile()
        {
            CreateMap<UsuarioEmpresa, EmpresaModelResponse>()
                .ForMember(x => x.Sigla, opt => opt.MapFrom(x => x.Empresa.Sigla))
                .ForMember(x => x.IdEmpresa, opt => opt.MapFrom(x => x.Empresa.IdEmpresa));
        }
    }
}
