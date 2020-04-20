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
    public class FreteTipoService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public FreteTipoService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task ConsultarFreteTipo()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            string where = "WHERE NUCAMPO IN (SELECT NUCAMPO FROM TDDCAM WHERE NOMETAB = 'TGFCAB' AND NOMECAMPO = 'CIF_FOB')";

            List<FreteTipoIntegracao> freteTiposIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<FreteTipoIntegracao>(where: where);

            foreach (var freteTipoInt in freteTiposIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(freteTipoInt);

                    bool freteTipoNovo = false;

                    FreteTipo freteTipo = _uow.FreteTipoRepository.ConsultarPorSigla(freteTipoInt.Sigla);

                    if (freteTipo == null)
                    {
                        freteTipoNovo = true;
                        freteTipo = new FreteTipo();
                    }

                    freteTipo.Descricao = freteTipoInt.Descricao;
                    freteTipo.Sigla = freteTipoInt.Sigla;

                    if (freteTipoNovo)
                    {
                        _uow.FreteTipoRepository.Add(freteTipo);
                    }
                    else
                    {
                        _uow.FreteTipoRepository.Update(freteTipo);
                    }

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração do tipo de frete: {0}.", freteTipoInt.Sigla), ex);
                }
            }
        }
    }
}
