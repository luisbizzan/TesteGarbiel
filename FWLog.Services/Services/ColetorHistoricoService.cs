using FWLog.Data.Models;
using FWLog.Data.Repository.GeneralCtx;
using FWLog.Services.Model.Coletor;
using System;

namespace FWLog.Services.Services
{
    public class ColetorHistoricoService
    {
        private readonly ColetorHistoricoTipoRepository _coletorHistoricoTipoRepository;

        public ColetorHistoricoService(ColetorHistoricoTipoRepository coletorHistoricoTipo)
        {
            _coletorHistoricoTipoRepository = coletorHistoricoTipo;
        }

        public void GravarHistoricoColetor(GravarHistoricoColetorRequisicao gravarHistoricoColetorRequisicao)
        {
            var coletorHistorico = new ColetorHistorico
            {
                IdColetorAplicacao = gravarHistoricoColetorRequisicao.IdColetorAplocacao,
                IdColetorHistoricoTipo = gravarHistoricoColetorRequisicao.IdColetorHistoricoTipo,
                DataHora = DateTime.Now,
                Descricao = gravarHistoricoColetorRequisicao.Descricao,
                IdEmpresa = gravarHistoricoColetorRequisicao.IdEmpresa,
                IdUsuario = gravarHistoricoColetorRequisicao.IdUsuario
            };

            _coletorHistoricoTipoRepository.GravarHistorico(coletorHistorico);
        }
    }
}
