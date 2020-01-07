using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class EmpresaService : BaseService
    {
        private UnitOfWork _unitOfWork;

        public EmpresaService(UnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task ConsultarEmpresaIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder inner = new StringBuilder();

            inner.Append("INNER JOIN TGFEMP ON TSIEMP.CODEMP = TGFEMP.CODEMP ");
            inner.Append("LEFT JOIN TSIEND ON TSIEMP.CODEND = TSIEND.CODEND ");
            inner.Append("LEFT JOIN TSIBAI ON TSIEMP.CODBAI = TSIBAI.CODBAI ");
            inner.Append("LEFT JOIN TSICID ON TSIEMP.CODCID = TSICID.CODCID ");
            inner.Append("LEFT JOIN TSIUFS ON TSICID.UF = TSIUFS.CODUF");
            
            var where = "WHERE AD_INTEGRARFWLOG = '1' "; 

            List<EmpresaIntegracao> empresasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<EmpresaIntegracao>(inner: inner.ToString(), where: where);

            foreach (var empInt in empresasIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(empInt);

                    bool empresaNova = false;

                    var codEmp = Convert.ToInt32(empInt.CodigoIntegracao);
                    EmpresaConfig empresaConfig = _unitOfWork.EmpresaConfigRepository.ConsultaPorCodigoIntegracao(codEmp);

                    if (empresaConfig == null)
                    {
                        empresaNova = true;
                        empresaConfig = new EmpresaConfig();
                        empresaConfig.Empresa = new Empresa();
                    }

                    empresaConfig.Empresa.CodigoIntegracao = codEmp;
                    empresaConfig.Empresa.CEP = empInt.CEP;
                    empresaConfig.Empresa.Ativo = empInt.Ativo == "S" ? true : false;
                    empresaConfig.Empresa.Bairro = empInt.Bairro;
                    empresaConfig.Empresa.Cidade = empInt.Cidade;
                    empresaConfig.Empresa.CNPJ = empInt.CNPJ;
                    empresaConfig.Empresa.Complemento = empInt.Complemento;
                    empresaConfig.Empresa.Endereco = empInt.Endereco;
                    empresaConfig.Empresa.Estado = empInt.Estadp;
                    empresaConfig.Empresa.NomeFantasia = empInt.NomeFantasia;
                    empresaConfig.Empresa.Numero = empInt.Numero;
                    empresaConfig.Empresa.RazaoSocial = empInt.RazaoSocial;
                    empresaConfig.Empresa.Sigla = empInt.Sigla == null ? empInt.NomeFantasia.Substring(0, 3) : empInt.Sigla;//TODO temporário
                    empresaConfig.Empresa.Telefone = empInt.Telefone;
                    empresaConfig.Empresa.TelefoneSAC = empInt.TelefoneSAC;
                    empresaConfig.IdEmpresaTipo = empInt.EmpresaMatriz == empInt.CodigoIntegracao ? EmpresaTipoEnum.Matriz : EmpresaTipoEnum.Filial;
                  
                    if (empresaNova)
                    {
                        _unitOfWork.EmpresaConfigRepository.Add(empresaConfig);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(empInt.EmpresaMatriz))
                    {
                        var codEmpMatriz = Convert.ToInt32(empInt.EmpresaMatriz);
                        var empMatriz = _unitOfWork.EmpresaRepository.Tabela().FirstOrDefault(f => f.CodigoIntegracao == codEmpMatriz);

                        if (empMatriz != null)
                        {
                            empresaConfig.IdEmpresaMatriz = empMatriz.IdEmpresa;

                            await _unitOfWork.SaveChangesAsync();
                        }
                    }

                    bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Empresa", "CODEMP", codEmp, "AD_INTEGRARFWLOG", '0');

                    if (!atualizacaoOK)
                    {
                        throw new Exception("A atualização de Empresa no Sankhya não terminou com sucesso.");
                    }
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_unitOfWork);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração da seguinte Empresa: {0}.", empInt.CodigoIntegracao));

                    continue;
                }
            }
        }

        public void Editar(EmpresaConfig empresaConfig)
        {
            _unitOfWork.EmpresaConfigRepository.Update(empresaConfig);
            _unitOfWork.SaveChanges();
        }
    }
}
