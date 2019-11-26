using AutoMapper;
using FWLog.Services.Model;
using FWLog.Web.Api.Models.Usuario;

namespace FWLog.Web.Api.Mapping
{
    public class LoginModelResponseProfile : Profile
    {
        public LoginModelResponseProfile()
        {
            CreateMap<TokenResponse, LoginModelResponse>()
                .ForMember(x => x.Empresas, opt => opt.Ignore());
        }
    }
}
