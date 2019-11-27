using FWLog.Data;
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
            where.Append(string.Format("AND DTALTER > to_date('{0}', 'dd-mm-yyyy hh24:mi:ss')", DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss")));//Data do Log de sincronização

            List<TransportadoraIntegracao> transportadorasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryGenerico<TransportadoraIntegracao>(where: where.ToString());

            foreach (var transpInt in transportadorasIntegracao)
            {
                ValidarTransportadoraIntegracao(transpInt);

                bool transportadoraNova = false;

                var codParc = Convert.ToInt64(transpInt.CODPARC);
                Transportadora transportadora = _uow.TransportadoraRepository.ConsultarPorCodigoIntegracao(codParc);

                if (transportadora == null)
                {
                    transportadoraNova = true;
                    transportadora = new Transportadora();
                }

                transportadora.CodigoIntegracao = codParc;
                transportadora.Ativo = transpInt.ATIVO == "S" ? true : false;
                transportadora.CNPJ = transpInt.CGC_CPF;
                transportadora.NomeFantasia = transpInt.RAZAOSOCIAL;
                transportadora.RazaoSocial = transpInt.NOMEPARC;

                if (transportadoraNova)
                {
                    _uow.TransportadoraRepository.Add(transportadora);
                }

                await _uow.SaveChangesAsync();
            }
        }

        public void ValidarTransportadoraIntegracao(TransportadoraIntegracao transportadoraIntegracao)
        {
            ValidarCampo(transportadoraIntegracao.CODPARC, nameof(transportadoraIntegracao.CODPARC));
            ValidarCampo(transportadoraIntegracao.ATIVO, nameof(transportadoraIntegracao.ATIVO));
            ValidarCampo(transportadoraIntegracao.CGC_CPF, nameof(transportadoraIntegracao.CGC_CPF));
            ValidarCampo(transportadoraIntegracao.NOMEPARC, nameof(transportadoraIntegracao.NOMEPARC));
            ValidarCampo(transportadoraIntegracao.RAZAOSOCIAL, nameof(transportadoraIntegracao.RAZAOSOCIAL));
        }
    }
}
