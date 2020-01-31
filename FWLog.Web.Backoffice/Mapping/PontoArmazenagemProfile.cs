using AutoMapper;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Models.PontoArmazenagemCtx;
using System;

namespace FWLog.Web.Backoffice.Mapping
{
    public class PontoArmazenagemProfile : Profile
    {
        public PontoArmazenagemProfile()
        {
            CreateMap<PontoArmazenagem, PontoArmazenagemEditarViewModel>()
                .ForMember(dest => dest.DescricaoNivelArmazenagem, opt => opt.MapFrom(src => src.NivelArmazenagem.Descricao))
                .ForMember(dest => dest.LimitePesoVertical, opt => opt.MapFrom(src => src.LimitePesoVertical.HasValue ? src.LimitePesoVertical.Value.ToString("n2") : string.Empty));

            CreateMap<PontoArmazenagemEditarViewModel, PontoArmazenagem>();

            CreateMap<PontoArmazenagem, PontoArmazenagemDetalhesViewModel>()
                .ForMember(dest => dest.NivelArmazenagem, opt => opt.MapFrom(src => src.NivelArmazenagem.Descricao))
                .ForMember(dest => dest.PontoArmazenagem, opt => opt.MapFrom(src => src.Descricao))
                .ForMember(dest => dest.LimitePesoVertical, opt => opt.MapFrom(src => src.LimitePesoVertical))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Ativo ? "Ativo" : "Inativo"))
                .ForMember(dest => dest.TipoArmazenagem, opt => opt.MapFrom(src => src.TipoArmazenagem.Descricao))
                .ForMember(dest => dest.TipoMovimentacao, opt => opt.MapFrom(src => src.TipoMovimentacao.Descricao));

            CreateMap<PontoArmazenagemPesquisaModalFiltroViewModel, PontoArmazenagemPesquisaModalFiltro>();
            CreateMap<PontoArmazenagemPesquisaModalListaLinhaTabela, PontoArmazenagemPesquisaModalItemViewModel>();

            CreateMap<PontoArmazenagemListaLinhaTabela, PontoArmazenagemListaItemViewModel>()
                .ForMember(dest => dest.LimitePesoVertical, opt => opt.MapFrom(src => src.LimitePesoVertical.HasValue ? src.LimitePesoVertical.Value.ToString("n2").Replace('.',',') : string.Empty));
        }
    }
}