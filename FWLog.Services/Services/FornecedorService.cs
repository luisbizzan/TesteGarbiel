using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class FornecedorService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public FornecedorService(UnitOfWork uow, ILog log)
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

            StringBuilder inner = new StringBuilder();
            //inner.Append("INNER JOIN TGFEMP ON TGFPAR.CODEMP = TGFEMP.CODEMP ");
            inner.Append("LEFT JOIN TSIEND ON TGFPAR.CODEND  = TSIEND.CODEND ");
            inner.Append("LEFT JOIN TSIBAI ON TGFPAR.CODBAI  = TSIBAI.CODBAI ");
            inner.Append("LEFT JOIN TSICID ON TGFPAR.CODCID  = TSICID.CODCID ");
            inner.Append("LEFT JOIN TSIUFS ON TSICID.UF      = TSIUFS.CODUF");

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");
            where.Append("TGFPAR.CGC_CPF IS NOT NULL ");
            where.Append("AND TGFPAR.RAZAOSOCIAL IS NOT NULL ");
            where.Append("AND TGFPAR.FORNECEDOR = 'S' ");
            where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '0' ");

            List<FornecedorIntegracao> fornecedoresIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<FornecedorIntegracao>(where.ToString(), inner.ToString());

            foreach (var fornecInt in fornecedoresIntegracao)
            {
                try
                {
                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", fornecInt.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", '1');
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na limpeza de integração do Fornecedor: {0}.", fornecInt.CodigoIntegracao), ex);
                }
            }
        }

        public async Task ConsultarFornecedor()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder inner = new StringBuilder();
            //inner.Append("INNER JOIN TGFEMP ON TGFPAR.CODEMP = TGFEMP.CODEMP ");
            inner.Append("LEFT JOIN TSIEND ON TGFPAR.CODEND  = TSIEND.CODEND ");
            inner.Append("LEFT JOIN TSIBAI ON TGFPAR.CODBAI  = TSIBAI.CODBAI ");
            inner.Append("LEFT JOIN TSICID ON TGFPAR.CODCID  = TSICID.CODCID ");
            inner.Append("LEFT JOIN TSIUFS ON TSICID.UF      = TSIUFS.CODUF");

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");
            where.Append("TGFPAR.CGC_CPF IS NOT NULL ");
            where.Append("AND TGFPAR.RAZAOSOCIAL IS NOT NULL ");
            where.Append("AND TGFPAR.FORNECEDOR = 'S' ");
            where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");

            List<FornecedorIntegracao> fornecedoresIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<FornecedorIntegracao>(where.ToString(), inner.ToString());

            foreach (var fornecInt in fornecedoresIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(fornecInt);

                    bool fornecedorNovo = false;

                    var codParc = Convert.ToInt64(fornecInt.CodigoIntegracao);
                    Fornecedor fornecedor = _uow.FornecedorRepository.ConsultarPorCodigoIntegracao(codParc);

                    if (fornecedor == null)
                    {
                        fornecedorNovo = true;
                        fornecedor = new Fornecedor();
                    }

                    fornecedor.CodigoIntegracao = codParc;
                    fornecedor.Ativo = fornecInt.Ativo == "S" ? true : false;
                    fornecedor.CNPJ = fornecInt.CNPJ;
                    fornecedor.RazaoSocial = fornecInt.RazaoSocial;
                    fornecedor.NomeFantasia = fornecInt.NomeFantasia;
                    fornecedor.CEP = fornecInt.CEP;
                    fornecedor.Bairro = fornecInt.Bairro;
                    fornecedor.Cidade = fornecInt.Cidade;
                    fornecedor.Complemento = fornecInt.Complemento;
                    fornecedor.Endereco = fornecInt.Endereco;
                    fornecedor.Estado = fornecInt.Estado;
                    fornecedor.Numero = fornecInt.Numero;
                    fornecedor.Telefone = fornecInt.Telefone;

                    if (fornecedorNovo)
                    {
                        _uow.FornecedorRepository.Add(fornecedor);
                    }
                    else
                    {
                        _uow.FornecedorRepository.Update(fornecedor);
                    }

                    _uow.SaveChanges();

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", fornecedor.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", '0');
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração do Fornecedor: {0}.", fornecInt.CodigoIntegracao), ex);
                }
            }
        }
    }
}
