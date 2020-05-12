using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using FWLog.Services.Model.Transportadora;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class TransportadoraService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public TransportadoraService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task LimparIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");
            where.Append("CGC_CPF IS NOT NULL ");
            where.Append("AND RAZAOSOCIAL IS NOT NULL ");
            where.Append("AND TRANSPORTADORA = 'S' ");
            where.Append("AND AD_INTEGRARFWLOG = '0' ");

            List<TransportadoraIntegracao> transportadorasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<TransportadoraIntegracao>(where: where.ToString());

            foreach (var transpInt in transportadorasIntegracao)
            {
                Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", transpInt.CodigoIntegracao.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", "1");
            }
        }

        public async Task ConsultarTransportadora()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");
            where.Append("CGC_CPF IS NOT NULL ");
            where.Append("AND RAZAOSOCIAL IS NOT NULL ");
            where.Append("AND TRANSPORTADORA = 'S' ");
            where.Append("AND AD_INTEGRARFWLOG = '1' ");
            where.Append("AND AD_ABREVTRANSP IS NOT NULL ");

            List<TransportadoraIntegracao> transportadorasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<TransportadoraIntegracao>(where: where.ToString());

            foreach (var transpInt in transportadorasIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(transpInt);

                    bool transportadoraNova = false;

                    var codParc = Convert.ToInt64(transpInt.CodigoIntegracao);
                    Transportadora transportadora = _uow.TransportadoraRepository.ConsultarPorCodigoIntegracao(codParc);

                    if (transportadora == null)
                    {
                        transportadoraNova = true;
                        transportadora = new Transportadora();
                    }

                    transportadora.CodigoIntegracao = codParc;
                    transportadora.Ativo = transpInt.Ativo == "S" ? true : false;
                    transportadora.CNPJ = transpInt.CNPJ;
                    transportadora.RazaoSocial = transpInt.RazaoSocial;
                    transportadora.NomeFantasia = transpInt.NomeFantasia;
                    transportadora.CodigoTransportadora = transpInt.CodigoTransportadora;

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", transportadora.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", "0");

                    if (transportadoraNova)
                    {
                        _uow.TransportadoraRepository.Add(transportadora);
                    }
                    else
                    {
                        _uow.TransportadoraRepository.Update(transportadora);
                    }

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração da Transportadora: {0}.", transpInt.CodigoIntegracao), ex);

                    continue;
                }
            }
        }

        public ConsultaTransportadoraResposta ConsultarTransportadora(string codigoTransportadora)
        {
            var transportadora = _uow.TransportadoraRepository.ConsultarPorCodigoTransportadora(codigoTransportadora);

            if (transportadora != null)
            {
                return new ConsultaTransportadoraResposta
                {
                    IdTransportadora = transportadora.IdTransportadora,
                    Nome = transportadora.NomeFantasia,
                    CodigoTransportadora = transportadora.CodigoTransportadora
                };
            }

            return null;
        }
    }
}