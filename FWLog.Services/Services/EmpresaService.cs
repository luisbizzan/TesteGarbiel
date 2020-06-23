using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using log4net;
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
        private ILog _log;
        private readonly ClienteService _clienteService;

        public EmpresaService(UnitOfWork uow, ILog log, ClienteService clienteService)
        {
            _unitOfWork = uow;
            _log = log;
            _clienteService = clienteService;
        }

        public async Task ConsultarEmpresaIntegracao(bool somenteNovos = true)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var inner = new StringBuilder();
            inner.Append("INNER JOIN TGFEMP ON TSIEMP.CODEMP = TGFEMP.CODEMP ");
            inner.Append("LEFT JOIN TSIEND ON TSIEMP.CODEND = TSIEND.CODEND ");
            inner.Append("LEFT JOIN TSIBAI ON TSIEMP.CODBAI = TSIBAI.CODBAI ");
            inner.Append("LEFT JOIN TSICID ON TSIEMP.CODCID = TSICID.CODCID ");
            inner.Append("LEFT JOIN TSIUFS ON TSICID.UF = TSIUFS.CODUF ");

            var where = new StringBuilder();

            if (somenteNovos)
            {
                where.Append("WHERE TSIEMP.AD_INTEGRARFWLOG = '1' ");
            }

            List<EmpresaIntegracao> empresasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<EmpresaIntegracao>(where.ToString(), inner.ToString());

            empresasIntegracao = empresasIntegracao.OrderBy("CodigoIntegracao", "ASC").ToList();

            foreach (var empInt in empresasIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(empInt);

                    Cliente cliente = null;

                    if(empInt.CodigoParceiro != null && !empInt.CodigoParceiro.Equals("0"))
                    {
                        var codParc = Convert.ToInt32(empInt.CodigoParceiro);
                        cliente = _unitOfWork.ClienteRepository.ConsultarPorCodigoIntegracao(codParc);

                        if(cliente == null)
                        {
                            cliente = await _clienteService.ConsultarClientePorCodigoIntegracao(empInt.CodigoParceiro);
                        }
                    }

                    bool empresaNova = false;

                    var codEmp = Convert.ToInt32(empInt.CodigoIntegracao);
                    EmpresaConfig empresaConfig = _unitOfWork.EmpresaConfigRepository.ConsultaPorCodigoIntegracao(codEmp);

                    if (empresaConfig == null)
                    {
                        empresaNova = true;
                        empresaConfig = new EmpresaConfig
                        {
                            Empresa = new Empresa()
                        };
                    }

                    empresaConfig.Empresa.CodigoIntegracao = codEmp;
                    empresaConfig.Empresa.CEP = empInt.CEP;
                    empresaConfig.Empresa.Ativo = empInt.Ativo == "S" ? true : false;
                    empresaConfig.Empresa.Bairro = empInt.Bairro;
                    empresaConfig.Empresa.Cidade = empInt.Cidade;
                    empresaConfig.Empresa.CNPJ = empInt.CNPJ;
                    empresaConfig.Empresa.Complemento = empInt.Complemento;
                    empresaConfig.Empresa.Endereco = empInt.Endereco;
                    empresaConfig.Empresa.Estado = empInt.Estado;
                    empresaConfig.Empresa.NomeFantasia = empInt.NomeFantasia;
                    empresaConfig.Empresa.Numero = empInt.Numero;
                    empresaConfig.Empresa.RazaoSocial = empInt.RazaoSocial;
                    empresaConfig.Empresa.Sigla = empInt.Sigla;
                    empresaConfig.Empresa.Telefone = empInt.Telefone;
                    empresaConfig.Empresa.TelefoneSAC = empInt.TelefoneSAC;
                    empresaConfig.IdEmpresaTipo = empInt.EmpresaMatriz == empInt.CodigoIntegracao ? EmpresaTipoEnum.Matriz : EmpresaTipoEnum.Filial;
                    empresaConfig.Empresa.IdCliente = cliente?.IdCliente;

                    if (empresaNova)
                    {
                        _unitOfWork.EmpresaConfigRepository.Add(empresaConfig);
                    }
                    else
                    {
                        _unitOfWork.EmpresaRepository.Update(empresaConfig.Empresa);
                        _unitOfWork.SaveChanges();
                        _unitOfWork.EmpresaConfigRepository.Update(empresaConfig);
                    }

                    _unitOfWork.SaveChanges();

                    if (!string.IsNullOrEmpty(empInt.EmpresaMatriz) && !empInt.EmpresaMatriz.Equals(codEmp.ToString()))
                    {
                        var codEmpMatriz = Convert.ToInt32(empInt.EmpresaMatriz);
                        var empMatriz = _unitOfWork.EmpresaRepository.Tabela().FirstOrDefault(f => f.CodigoIntegracao == codEmpMatriz);

                        if (empMatriz != null)
                        {
                            empresaConfig.IdEmpresaMatriz = empMatriz.IdEmpresa;
                            _unitOfWork.EmpresaConfigRepository.Update(empresaConfig);
                            _unitOfWork.SaveChanges();
                        }
                    }

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODEMP", empresaConfig.Empresa.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Empresa", campoChave, "TSIEMP.AD_INTEGRARFWLOG", "0");
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração da Empresa: {0}.", empInt.CodigoIntegracao), ex);
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
