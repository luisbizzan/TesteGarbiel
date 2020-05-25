using DartDigital.Library.Exceptions;
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
                    {"AD_ROMANEIO.NUMROMANEIO", nroRomaneio },
                    {"AD_ROMANEIO.AD_DTEMBROMANEIO",dataHoraRomaneio }
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
    }
}
