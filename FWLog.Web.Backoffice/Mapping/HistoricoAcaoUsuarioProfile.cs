using AutoMapper;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Relatorio.Model;
using FWLog.Web.Backoffice.Models.HistoricoAcaoUsuarioCtx;

namespace FWLog.Web.Backoffice.Mapping
{
    public class HistoricoAcaoUsuarioProfile : Profile
    {
        public HistoricoAcaoUsuarioProfile()
        {
            CreateMap<DownloadHistoricoAcaoUsuarioViewModel, RelatorioHistoricoAcaoUsuarioRequest>();
            CreateMap<ImprimirHistoricoAcaoUsuarioViewModel, ImprimirRelatorioHistoricoAcaoUsuarioRequest>();
        }
    }
}