using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Coletor;
using log4net;
using System;

namespace FWLog.Services.Services
{
    public class ColetorHistoricoService
    {
        private readonly ILog _log;
        private readonly UnitOfWork _unitOfWork;

        public ColetorHistoricoService(UnitOfWork unitOfWork, ILog log)
        {
            _unitOfWork = unitOfWork;
            _log = log;
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

                _unitOfWork.ColetorHistoricoTipoRepository.GravarHistorico(coletorHistorico);
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
