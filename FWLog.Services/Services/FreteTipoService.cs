﻿using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class FreteTipoService : BaseService
    {
        private UnitOfWork _uow;

        public FreteTipoService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultarFreteTipo()
        {
            //if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            //{
            //    return;
            //}

            StringBuilder where = new StringBuilder();
            where.Append("WHERE NUCAMPO IN (SELECT NUCAMPO FROM TDDCAM WHERE NOMETAB = 'TGFCAB' AND NOMECAMPO = 'CIF_FOB')");            

            List<FreteTipoIntegracao> freteTiposIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryGenerico<FreteTipoIntegracao>(where: where.ToString());

            foreach (var freteTipoInt in freteTiposIntegracao)
            {
                try
                {
                    ValidarFreteTipoIntegracao(freteTipoInt);

                    bool freteTipoNovo = false;
                                        
                    FreteTipo freteTipo = _uow.FreteTipoRepository.ConsultarPorSigla(freteTipoInt.VALOR);

                    if (freteTipo == null)
                    {
                        freteTipoNovo = true;
                        freteTipo = new FreteTipo();
                    }

                    freteTipo.Descricao = freteTipoInt.OPCAO;
                    freteTipo.Sigla = freteTipoInt.VALOR;

                    if (freteTipoNovo)
                    {
                        _uow.FreteTipoRepository.Add(freteTipo);
                    }

                    await _uow.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração do seguinte tipo de frete: {0}.", freteTipoInt.VALOR));

                    continue;
                }
            }
        }

        public void ValidarFreteTipoIntegracao(FreteTipoIntegracao freteTipoIntegracao)
        {
            ValidarCampo(freteTipoIntegracao.OPCAO, nameof(freteTipoIntegracao.OPCAO));
            ValidarCampo(freteTipoIntegracao.VALOR, nameof(freteTipoIntegracao.VALOR));
        }
    }
}
