using AutoMapper;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Models.PerfilImpressoraCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Web.Backoffice.Mapping
{
    public class PerfilImpressoraProfile : Profile
    {
        public PerfilImpressoraProfile()
        {
            CreateMap<PerfilImpressoraTableRow, PerfilImpressoraListItemViewModel>();

            CreateMap<PerfilImpressora, PerfilImpressoraCreateViewModel>()
                  .ForMember(dest => dest.TiposImpressao, opt => opt.MapFrom(src => MappingTipoImpressaoViewModel(src.PerfilImpressoraItens)));

            CreateMap<PerfilImpressoraCreateViewModel, PerfilImpressora>()
                .ForMember(dest => dest.PerfilImpressoraItens, opt => opt.MapFrom(src => MappingPerfilImpressoraItem(src.TiposImpressao, src.IdPerfilImpressora))); ;

            CreateMap<PerfilImpressora, PerfilImpressoraDetailsViewModel>();

            CreateMap<PerfilImpressoraFilterViewModel, PerfilImpressoraFilter>();
        }

        public List<TipoImpressaoViewModel> MappingTipoImpressaoViewModel(ICollection<PerfilImpressoraItem> perfilImpressoraItens)
        {
            List<TipoImpressaoViewModel> tiposImpressao = new List<TipoImpressaoViewModel>();

            var agrupado = perfilImpressoraItens.GroupBy(g => g.IdImpressaoItem).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var item in agrupado)
            {
                List<ImpressoraViewModel> impressorasView = new List<ImpressoraViewModel>();

                foreach (var impressora in item.Value)
                {
                    var impressoraView = new ImpressoraViewModel()
                    {
                        IdImpressora = impressora.IdImpressora,
                        Nome = impressora.Impressora.Name,
                        Selecionado = true
                    };

                    impressorasView.Add(impressoraView);
                }

                var TipoImpressaoViewModel = new TipoImpressaoViewModel()
                {
                    Descricao = item.Value.First().ImpressaoItem.Descricao,
                    IdImpressaoItem = item.Key.GetHashCode(),
                    Impressoras = impressorasView                   
                };

                tiposImpressao.Add(TipoImpressaoViewModel);
            }

            return tiposImpressao;
        }

        public List<PerfilImpressoraItem> MappingPerfilImpressoraItem(List<TipoImpressaoViewModel> tiposImpressoes, long idPerfilImpressora)
        {
            List<PerfilImpressoraItem> perfilImpressoraItens = new List<PerfilImpressoraItem>();

            foreach (var tipoImpressao in tiposImpressoes)
            {
                foreach (var impressora in tipoImpressao.Impressoras)
                {
                    if (!impressora.Selecionado)
                    {
                        continue;
                    }

                    PerfilImpressoraItem perfilImpressoraItem = new PerfilImpressoraItem()
                    {
                        IdImpressaoItem = (ImpressaoItemEnum)tipoImpressao.IdImpressaoItem,
                        IdImpressora = impressora.IdImpressora,
                        IdPerfilImpressora = idPerfilImpressora
                    };

                    perfilImpressoraItens.Add(perfilImpressoraItem);
                }
            }

            return perfilImpressoraItens;
        }
    }
}
