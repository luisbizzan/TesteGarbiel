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

            inner.Append("INNER JOIN TGFEMP ON TSIEMP.CODEMP = TGFEMP.CODEMP");
            inner.Append("LEFT JOIN TSIEND ON TSIEMP.CODEND = TSIEND.CODEND ");
            inner.Append("LEFT JOIN TSIBAI ON TSIEMP.CODBAI = TSIBAI.CODBAI ");
            inner.Append("LEFT JOIN TSICID ON TSIEMP.CODCID = TSICID.CODCID ");
            inner.Append("LEFT JOIN TSIUFS ON TSICID.UF = TSIUFS.CODUF");

            List<EmpresaIntegracao> empresasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryComplexa<EmpresaIntegracao>(inner: inner.ToString());

            foreach (var empInt in empresasIntegracao)
            {
                try
                {
                    ValidarEmpresaIntegracao(empInt);

                    bool empresaNova = false;

                    var codEmp = Convert.ToInt32(empInt.CODEMP);
                    Empresa empresa = _unitOfWork.EmpresaRepository.ConsultaPorCodigoIntegracao(codEmp);

                    if (empresa == null)
                    {
                        empresaNova = true;
                        empresa = new Empresa();
                    }
                    empresa.CodigoIntegracao = codEmp;
                    empresa.CEP = empInt.CEP;
                    empresa.Ativo = empInt.ATIVO == "S" ? true : false;
                    empresa.Bairro = empInt.NOMEBAI;
                    empresa.Cidade = empInt.NOMECID;
                    empresa.CNPJ = empInt.CGC;
                    empresa.Complemento = empInt.COMPLEMENTO;
                    empresa.Endereco = empInt.NOMEEND;
                    empresa.Estado = empInt.ESTADO;
                    empresa.NomeFantasia = empInt.NOMEFANTASIA;
                    empresa.Numero = empInt.NUMEND;
                    empresa.RazaoSocial = empInt.RAZAOSOCIAL;
                    empresa.Sigla = empInt.AD_UNIDABREV == null ? empInt.NOMEFANTASIA.Substring(0, 3) : empInt.AD_UNIDABREV;//TODO temporário
                    empresa.Telefone = empInt.TELEFONE;
                   //TODO empresa.EmpresaConfig.IdEmpresaTipo = empInt.CODEMPMATRIZ == empInt.CODEMP ? EmpresaTipoEnum.Matriz : EmpresaTipoEnum.Filial;
                    
                    if (empresaNova)
                    {
                        _unitOfWork.EmpresaRepository.Add(empresa);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(empInt.CODEMPMATRIZ))
                    {
                        var codEmpMatriz = Convert.ToInt32(empInt.CODEMPMATRIZ);
                        var empMatriz = _unitOfWork.EmpresaRepository.Tabela().FirstOrDefault(f => f.CodigoIntegracao == codEmpMatriz);

                        if (empMatriz != null)
                        {
                           // empresa.EmpresaConfig.IdEmpresaMatriz = empMatriz.IdEmpresa;

                            await _unitOfWork.SaveChangesAsync();
                        }
                    }

                    bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Empresa", "CODEMP", empresa.CodigoIntegracao, "DTALTER", DateTime.UtcNow);

                    if (!atualizacaoOK)
                    {
                        throw new Exception("A atualização de Empresa no Sankhya não terminou com sucesso.");
                    }
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_unitOfWork);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração da seguinte Empresa: {0}.", empInt.CODEMP));

                    continue;
                }
            }
        }

        public void ValidarEmpresaIntegracao(EmpresaIntegracao empresaIntegracao)
        {
            ValidarCampo(empresaIntegracao.CODEMP, nameof(empresaIntegracao.ATIVO));
        }

        public void Editar(EmpresaConfig empresaConfig)
        {
            _unitOfWork.EmpresaConfigRepository.Update(empresaConfig);
            _unitOfWork.SaveChanges();
        }
    }
}
