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
        private UnitOfWork _uow;

        public EmpresaService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultarEmpresaIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder inner = new StringBuilder();

            inner.Append("LEFT JOIN TSIEND ON TSIEMP.CODEND = TSIEND.CODEND ");
            inner.Append("LEFT JOIN TSIBAI ON TSIEMP.CODBAI = TSIBAI.CODBAI ");
            inner.Append("LEFT JOIN TSICID ON TSIEMP.CODCID = TSICID.CODCID ");
            inner.Append("LEFT JOIN TSIUFS ON TSICID.UF = TSIUFS.CODUF");

            List<EmpresaIntegracao> empresasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryComplexa<EmpresaIntegracao>(inner: inner.ToString());

            foreach (var empInt in empresasIntegracao)
            {
                ValidarEmpresaIntegracao(empInt);

                bool empresaNova = false;

                var codEmp = Convert.ToInt32(empInt.CODEMP);
                Empresa empresa = _uow.EmpresaRepository.ConsultaPorCodigoIntegracao(codEmp);

                if (empresa == null)
                {
                    empresaNova = true;
                    empresa = new Empresa();
                }

                empresa.CodigoIntegracao = codEmp;
                empresa.CEP = string.IsNullOrEmpty(empInt.CEP) ? (int?)null : Convert.ToInt32(empInt.CEP);
                empresa.Ativo = empInt.AD_ATIVOSAV == "S" ? true : false;
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
                empresa.IdEmpresaTipo = empInt.CODEMPMATRIZ == empInt.CODEMP ? EmpresaTipoEnum.Matriz.GetHashCode() : EmpresaTipoEnum.Filial.GetHashCode();

                try
                {
                    if (empresaNova)
                    {
                        _uow.EmpresaRepository.Add(empresa);
                    }

                    await _uow.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(empInt.CODEMPMATRIZ))
                    {
                        var empMatriz = _uow.EmpresaRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao.ToString() == empInt.CODEMPMATRIZ);

                        if (empMatriz != null)
                        {
                            empresa.IdEmpresaMatriz = empMatriz.IdEmpresa;

                            await _uow.SaveChangesAsync();
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
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex);

                    continue;
                }
            }
        }

        public void ValidarEmpresaIntegracao(EmpresaIntegracao empresaIntegracao)
        {
            ValidarCampo(empresaIntegracao.CODEMP, nameof(empresaIntegracao.AD_ATIVOSAV));
        }
    }
}
