using DartDigital.Library.Exceptions;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class RomaneioService : BaseService
    {
        private ILog _log;

        public RomaneioService(ILog log)
        {
            _log = log;
        }

        public async Task InserirRomaneioSankhya(int nroRomaneio, DateTime dataHoraRomaneio)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            try
            {
                Dictionary<string, object> fields = new Dictionary<string, object>
                {
                    {"NUMROMANEIO", nroRomaneio },
                    {"DTEMBROMANEIO",dataHoraRomaneio.ToString("dd/MM/yyyy") }
                };

                await IntegracaoSankhya.Instance.InserirInformacaoIntegracao("AD_ROMANEIO", fields);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Erro na inserção do romaneio: {0}.", nroRomaneio);
                _log.Error(errorMessage, ex);
                throw new BusinessException(errorMessage);
            }
        }

        public async Task AtualizarRomaneioNotaFiscal(Pedido pedido, int nroRomaneio)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            try
            {
                Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedido.CodigoIntegracaoNotaFiscal.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_NUMROMANEIO", nroRomaneio);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Erro na atualização da nota fiscal de venda: {0}.", pedido.CodigoIntegracaoNotaFiscal);
                _log.Error(errorMessage, ex);
                throw new BusinessException(errorMessage);
            }
        }
    }
}
