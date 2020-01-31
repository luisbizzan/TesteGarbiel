using FWLog.Data;
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
    public class TransportadoraService : BaseService
    {
        private UnitOfWork _uow;

        public TransportadoraService(UnitOfWork uow)
        {
            _uow = uow;
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

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", transportadora.CodigoIntegracao.ToString() } };

                    bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", "0");
                    
                    if (!atualizacaoOK)
                    {
                        throw new Exception("A atualização de Transportadora no Sankhya não terminou com sucesso.");
                    }

                    if (transportadoraNova)
                    {
                        _uow.TransportadoraRepository.Add(transportadora);
                    }

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro na integração da Transportadora: {0}.", transpInt.CodigoIntegracao));

                    continue;
                }
            }
        }
    }
}
