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

        public async Task ConsultarFornecedor(bool somenteNovos = true)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = new StringBuilder();
            where.Append("WHERE TGFPAR.FORNECEDOR = 'S' ");

            if (somenteNovos)
            {
                where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");
            }
            else
            {
                where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '0' ");
            }

            int quantidadeChamadas = 0;

            var fornecedorContadorIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<FornecedorContadorIntegracao>(where: where.ToString());

            if (fornecedorContadorIntegracao != null)
            {
                try
                {
                    decimal contadorRegistros = Convert.ToInt32(fornecedorContadorIntegracao[0].Quantidade);

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
                    _log.Error("Erro na integração de Fornecedor", ex);
                    return;
                }
            }

            int offsetRows = 0;
            var fornecedoresIntegracao = new List<FornecedorIntegracao>();
            var inner = new StringBuilder();
            inner.Append("LEFT JOIN TSIEND ON TGFPAR.CODEND = TSIEND.CODEND ");
            inner.Append("LEFT JOIN TSIBAI ON TGFPAR.CODBAI = TSIBAI.CODBAI ");
            inner.Append("LEFT JOIN TSICID ON TGFPAR.CODCID = TSICID.CODCID ");
            inner.Append("LEFT JOIN TSIUFS ON TSICID.UF = TSIUFS.CODUF");

            for (int i = 0; i < quantidadeChamadas; i++)
            {
                where = new StringBuilder();
                where.Append("WHERE TGFPAR.FORNECEDOR = 'S' ");

                if (somenteNovos)
                {
                    where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");
                }
                else
                {
                    where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '0' ");
                }

                where.Append("ORDER BY TGFPAR.CODPARC ASC OFFSET " + offsetRows + " ROWS FETCH NEXT 4999 ROWS ONLY ");

                fornecedoresIntegracao.AddRange(await IntegracaoSankhya.Instance.PreExecutarQuery<FornecedorIntegracao>(where.ToString(), inner.ToString()));

                offsetRows += 4999;
            }

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

                    if (somenteNovos)
                    {
                        Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", fornecedor.CodigoIntegracao.ToString() } };
                        await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", '0');
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração do Fornecedor: {0}.", fornecInt.CodigoIntegracao), ex);
                }
            }
        }
    }
}
