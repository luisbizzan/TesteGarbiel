using AutoMapper;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.Models.CaixaRecusaCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Mapping
{
    public class CaixaRecusaProfile : Profile
    {
        public CaixaRecusaProfile()
        {
            CreateMap<CaixaRecusa, CaixaRecusaCadastroViewModel>()
                    .ForMember(c => c.DescricaoCaixa, opt => opt.Ignore())
                    .ForMember(c => c.DescricaoProduto, opt => opt.Ignore())
                    .ReverseMap();

            CreateMap<CaixaRecusa, CaixaRecusaEdicaoViewModel>()
                    .ForMember(c => c.DescricaoCaixa, opt => opt.Ignore())
                    .ForMember(c => c.DescricaoProduto, opt => opt.Ignore())
                    .ReverseMap();

            CreateMap<CaixaRecusa, CaixaRecusaEdicaoItemViewModel>()
                    .ForMember(c => c.DescricaoCaixa, opt => opt.MapFrom(x => x.Caixa.Nome))
                    .ForMember(c => c.DescricaoProduto, opt => opt.MapFrom(x => x.Produto.Descricao));
        }
    }
}