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
            where.Append(string.Format("AND DTALTER > to_date('{0}', 'dd-mm-yyyy hh24:mi:ss')", DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss")));//Data do Log de sincronização

            List<FornecedorIntegracao> fornecedoresIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryGenerico<FornecedorIntegracao>(where: where.ToString());

            foreach (var fornecInt in fornecedoresIntegracao)
            {
                try
                {
                    ValidarFornecedorIntegracao(fornecInt);

                    bool fornecedorNovo = false;

                    var codParc = Convert.ToInt64(fornecInt.CODPARC);
                    Fornecedor fornecedor = _uow.FornecedorRepository.ConsultarPorCodigoIntegracao(codParc);

                    if (fornecedor == null)
                    {
                        fornecedorNovo = true;
                        fornecedor = new Fornecedor();
                    }

                    fornecedor.CodigoIntegracao = codParc;
                    fornecedor.Ativo = fornecInt.ATIVO == "S" ? true : false;
                    fornecedor.CNPJ = fornecInt.CGC_CPF;
                    fornecedor.NomeFantasia = fornecInt.RAZAOSOCIAL;
                    fornecedor.RazaoSocial = fornecInt.NOMEPARC;

                    if (fornecedorNovo)
                    {
                        _uow.FornecedorRepository.Add(fornecedor);
                    }

                    await _uow.SaveChangesAsync();

                    bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", "CODPARC", fornecedor.CodigoIntegracao, "DTALTER", DateTime.UtcNow);
                    if (!atualizacaoOK)
                    {
                        throw new Exception("A atualização de Fornecedor no Sankhya não terminou com sucesso.");
                    }
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração do seguinte Fornecedor: {0}.", fornecInt.CODPARC));

                    continue;
                }
            }
        }

        public void ValidarFornecedorIntegracao(FornecedorIntegracao fornecedorIntegracao)
        {
            ValidarCampo(fornecedorIntegracao.CODPARC, nameof(fornecedorIntegracao.CODPARC));
            ValidarCampo(fornecedorIntegracao.ATIVO, nameof(fornecedorIntegracao.ATIVO));
            ValidarCampo(fornecedorIntegracao.CGC_CPF, nameof(fornecedorIntegracao.CGC_CPF));
            ValidarCampo(fornecedorIntegracao.NOMEPARC, nameof(fornecedorIntegracao.NOMEPARC));
            ValidarCampo(fornecedorIntegracao.RAZAOSOCIAL, nameof(fornecedorIntegracao.RAZAOSOCIAL));
        }
    }
}
