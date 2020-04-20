using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using log4net;

namespace FWLog.Services.Services
{
    public class UnidadeMedidaService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public UnidadeMedidaService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task ConsultarUnidadeMedida()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            List<UnidadeMedidaIntegracao> unidadesMedidaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<UnidadeMedidaIntegracao>();

            foreach (var unidadeInt in unidadesMedidaIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(unidadeInt);

                    bool unidadeNova = false;

                    UnidadeMedida unidadeMedida = _uow.UnidadeMedidaRepository.ConsultaPorSigla(unidadeInt.Sigla);

                    if (unidadeMedida == null)
                    {
                        unidadeNova = true;
                        unidadeMedida = new UnidadeMedida();
                    }

                    unidadeMedida.Sigla = unidadeInt.Sigla;
                    unidadeMedida.Descricao = unidadeInt.Descricao;

                    if (unidadeNova)
                    {
                        _uow.UnidadeMedidaRepository.Add(unidadeMedida);
                    }
                    else
                    {
                        _uow.UnidadeMedidaRepository.Update(unidadeMedida);
                    }

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração da unidade de medida: {0}.", unidadeInt.Sigla), ex);
                }
            }
        }
    }
}
