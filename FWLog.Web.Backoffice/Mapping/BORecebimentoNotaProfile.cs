using AutoMapper;
using FWLog.Services.Model.Lote;
using FWLog.Services.Model.Relatorios;
using FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class BORecebimentoNotaProfile : Profile
    {
        public BORecebimentoNotaProfile()
        {
            CreateMap<BODownloadRelatorioNotasViewModel, RelatorioRecebimentoNotasRequest>();
            CreateMap<TratarDivergenciaRecebimentoViewModel, TratarDivergenciaRequest>();
        }
    }
}