using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class UnidadeMedidaService : BaseService
    {
        private UnitOfWork _uow;

        public UnidadeMedidaService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultarUnidadeMedida()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            List<UnidadeMedidaIntegracao> unidadesMedidaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryGenerico<UnidadeMedidaIntegracao>();

            foreach (var unidadeInt in unidadesMedidaIntegracao)
            {
                try
                {
                    ValidarUnidadeMedidaIntegracao(unidadeInt);

                    bool unidadeNova = false;

                    UnidadeMedida unidadeMedida = _uow.UnidadeMedidaRepository.ConsultaPorSigla(unidadeInt.CODVOL);

                    if (unidadeMedida == null)
                    {
                        unidadeNova = true;
                        unidadeMedida = new UnidadeMedida();
                    }

                    unidadeMedida.Sigla = unidadeInt.CODVOL;
                    unidadeMedida.Descricao = unidadeInt.DESCRVOL;

                    if (unidadeNova)
                    {
                        _uow.UnidadeMedidaRepository.Add(unidadeMedida);
                    }

                    await _uow.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração da seguinte unidade de medida: {0}.", unidadeInt.CODVOL));

                    continue;
                }
            }
        }

        public void ValidarUnidadeMedidaIntegracao(UnidadeMedidaIntegracao unidadeMedidaIntegracao)
        {
            ValidarCampo(unidadeMedidaIntegracao.CODVOL, nameof(unidadeMedidaIntegracao.CODVOL));
            ValidarCampo(unidadeMedidaIntegracao.DESCRVOL, nameof(unidadeMedidaIntegracao.DESCRVOL));
        }
    }
}
