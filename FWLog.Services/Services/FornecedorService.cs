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
    public class FornecedorService : BaseService
    {
        private UnitOfWork _uow;

        public FornecedorService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultarFornecedor()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");
            where.Append("CGC_CPF IS NOT NULL ");
            where.Append("AND RAZAOSOCIAL IS NOT NULL ");
            where.Append("AND FORNECEDOR = 'S' ");
            where.Append("AND AD_INTEGRARFWLOG = '1' ");

            List<FornecedorIntegracao> fornecedoresIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<FornecedorIntegracao>(where: where.ToString());

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

                    if (fornecedorNovo)
                    {
                        _uow.FornecedorRepository.Add(fornecedor);
                    }

                    await _uow.SaveChangesAsync();

                    bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", "CODPARC", fornecedor.CodigoIntegracao, "AD_INTEGRARFWLOG", '0');
                    if (!atualizacaoOK)
                    {
                        throw new Exception("A atualização de Fornecedor no Sankhya não terminou com sucesso.");
                    }
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração do seguinte Fornecedor: {0}.", fornecInt.CodigoIntegracao));

                    continue;
                }
            }
        }
    }
}
