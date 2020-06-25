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
                    .ForMember(c => c.CaixaTipo, opt => opt.Ignore());

            CreateMap<Caixa, CaixaDetalhesViewModel>()
             .ForMember(caixa => caixa.CaixaTipoDescricao, opt => opt.MapFrom(src => src.CaixaTipo.Descricao))
             .ForMember(caixa => caixa.Ativo, opt => opt.MapFrom(src => src.Ativo ? "Sim" : "Não"));

            CreateMap<Caixa, CaixaEdicaoViewModel>()
                   .ForMember(caixa => caixa.ListaCaixaTipo, opt => opt.Ignore());

            CreateMap<CaixaEdicaoViewModel, Caixa>()
                   .ForMember(c => c.IdEmpresa, opt => opt.Ignore())
                   .ForMember(c => c.Cubagem, opt => opt.Ignore())
                   .ForMember(c => c.Empresa, opt => opt.Ignore())
                   .ForMember(c => c.CaixaTipo, opt => opt.Ignore());
        }
    }
}