using FWLog.Data.Models;
using FWLog.Data.Repository.GeneralCtx;
using FWLog.Services.Model.Coletor;
using log4net;
using System;

namespace FWLog.Services.Services
{
    public class ColetorHistoricoService
    {
        private readonly ColetorHistoricoTipoRepository _coletorHistoricoTipoRepository;
        private readonly ILog _log;

        public ColetorHistoricoService(ColetorHistoricoTipoRepository coletorHistoricoTipo, ILog ilog)
        {
            _coletorHistoricoTipoRepository = coletorHistoricoTipo;
            _log = ilog;
        }

        public void GravarHistoricoColetor(GravarHistoricoColetorRequisicao gravarHistoricoColetorRequisicao)
        {
            try
            {
                var coletorHistorico = new ColetorHistorico
                {
                    IdColetorAplicacao = gravarHistoricoColetorRequisicao.IdColetorAplicacao,
                    IdColetorHistoricoTipo = gravarHistoricoColetorRequisicao.IdColetorHistoricoTipo,
                    DataHora = DateTime.Now,
                    Descricao = gravarHistoricoColetorRequisicao.Descricao,
                    IdEmpresa = gravarHistoricoColetorRequisicao.IdEmpresa,
                    IdUsuario = gravarHistoricoColetorRequisicao.IdUsuario
                };

                _coletorHistoricoTipoRepository.GravarHistorico(coletorHistorico);
            }
            catch (Exception e)
            {
                try
                {
                    _log.Error("Erro ao gravar histórico de ações do usuário", e);
                }
                catch
                {
                  //Fazer nada.               
                }
            }
        }
    }
}
