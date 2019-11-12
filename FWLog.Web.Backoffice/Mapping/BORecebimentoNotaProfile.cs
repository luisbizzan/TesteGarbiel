using AutoMapper;
using FWLog.Data.Models;
using FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Mapping
{
    public class BORecebimentoNotaProfile : Profile
    {
        public BORecebimentoNotaProfile()
        {
            //CreateMap<Lote, BORecebimentoNotaListViewModel>()
            //    .ForMember(x => x.EmptyItem.Lote, op => op.MapFrom(x => x.IdLote))
            //    .ForMember(x => x.EmptyItem.Nota, op => op.MapFrom(x => x.NotaFiscal.Numero))
            //    .ForMember(x => x.EmptyItem.QuantidadePeca, op => op.MapFrom(x => x.QuantidadePeca))
            //    .ForMember(x => x.EmptyItem.QuantidadeVolume, op => op.MapFrom(x => x.QuantidadeVolume))
            //    .ForMember(x => x.EmptyItem.Status, op => op.MapFrom(x => x.LoteStatus.Descricao))
            //    .ForMember(x => x.EmptyItem.Fornecedor, op => op.MapFrom(x => x.NotaFiscal.Fornecedor.NomeFantasia));
        }
    }
}