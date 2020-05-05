using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace FWLog.Services.Services
{
    public class RepresentanteService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;
        private ILog _log;

        public RepresentanteService(UnitOfWork unitOfWork, ILog log) 
        {
            _unitOfWork = unitOfWork;
            _log = log;
        }

        public async Task ConsultarRepresentante()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");
            where.Append("TGFPAR.RAZAOSOCIAL IS NOT NULL ");
            where.Append("AND TGFPAR.VENDEDOR = 'S' ");
            where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");

            var inner = "INNER JOIN TGFVEN ON TGFVEN.CODPARC = TGFPAR.CODPARC";

            List<RepresentanteIntegracao> representantesIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<RepresentanteIntegracao>(where: where.ToString(), inner:inner.ToString());

            foreach (var representanteInt in representantesIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(representanteInt);

                    bool representanteNovo = false;

                    var codParc = Convert.ToInt64(representanteInt.CodigoIntegracao);
                    Representante representante = _unitOfWork.RepresentanteRepository.ConsultarPorCodigoIntegracao(codParc);

                    if (representante == null)
                    {
                        representanteNovo = true;
                        representante = new Representante();
                    }

                    representante.CodigoIntegracao = codParc;
                    representante.Ativo = representanteInt.Ativo == "S" ? true : false;
                    representante.Nome = representanteInt.RazaoSocial;
                    representante.CodigoIntegracaoVendedor = Convert.ToInt32(representanteInt.CodigoIntegracaoVendedor);

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", representante.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", "0");

                    if (representanteNovo)
                    {
                        _unitOfWork.RepresentanteRepository.Add(representante);
                    }
                    else
                    {
                        _unitOfWork.RepresentanteRepository.Update(representante);
                    }

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração da Representante: {0}.", representanteInt.CodigoIntegracao), ex);

                    continue;
                }
            }
        }
    }
}
