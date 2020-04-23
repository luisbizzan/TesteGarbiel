﻿using AutoMapper;
using FWLog.Data.Models;
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
                    .ForMember(c => c.Empresa, opt => opt.Ignore());
        }
    }
}