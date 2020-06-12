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

        public async Task ConsultarRepresentante(bool somenteNovos)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();
            where.Append("WHERE TGFPAR.VENDEDOR = 'S' ");

            if (somenteNovos)
            {
                where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");
            }
            else
            {
                where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '0' ");
            }

            string join = "INNER JOIN TGFVEN ON TGFVEN.CODPARC = TGFPAR.CODPARC ";

            int quantidadeChamadas = 0;

            var representanteContadorIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<RepresentanteContadorIntegracao>(where: where.ToString(), inner: join);

            if (representanteContadorIntegracao != null)
            {
                try
                {
                    decimal contadorRegistros = Convert.ToInt32(representanteContadorIntegracao[0].Quantidade);

                    if (contadorRegistros < 4999)
                    {
                        quantidadeChamadas = 1;
                    }
                    else
                    {
                        decimal div = contadorRegistros / 4999;
                        quantidadeChamadas = Convert.ToInt32(Math.Ceiling(div));
                    }

                }
                catch (Exception ex)
                {
                    _log.Error("Erro na integração de Representante", ex);
                    return;
                }
            }

            int offsetRows = 0;
            var representantesIntegracao = new List<RepresentanteIntegracao>();

            for (int i = 0; i < quantidadeChamadas; i++)
            {
                where = new StringBuilder();
                where.Append("WHERE TGFPAR.VENDEDOR = 'S' ");

                if (somenteNovos)
                {
                    where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");
                }
                else
                {
                    where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '0' ");
                }

                where.Append("ORDER BY TGFPAR.CODPARC ASC OFFSET " + offsetRows + " ROWS FETCH NEXT 4999 ROWS ONLY ");

                representantesIntegracao.AddRange(await IntegracaoSankhya.Instance.PreExecutarQuery<RepresentanteIntegracao>(where: where.ToString(), inner: join.ToString()));

                offsetRows += 4999;
            }

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

                    if (representanteNovo)
                    {
                        _unitOfWork.RepresentanteRepository.Add(representante);
                    }
                    else
                    {
                        _unitOfWork.RepresentanteRepository.Update(representante);
                    }

                    _unitOfWork.SaveChanges();

                    if (somenteNovos)
                    {
                        var campoChave = new Dictionary<string, string> { { "CODPARC", representante.CodigoIntegracao.ToString() } };
                        await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", "0");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração da Representante: {0}.", representanteInt.CodigoIntegracao), ex);
                }
            }
        }
    }
}
