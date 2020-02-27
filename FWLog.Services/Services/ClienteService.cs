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
    public class ClienteService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;

        public ClienteService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ConsultarCliente()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");
            where.Append("CGC_CPF IS NOT NULL ");
            //where.Append("CODVEND IS NOT NULL ");
            where.Append("AND RAZAOSOCIAL IS NOT NULL ");
            where.Append("AND CLIENTE = 'S' ");
            where.Append("AND AD_INTEGRARFWLOG = '1' ");

            List<ClienteIntegracao> clientesIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ClienteIntegracao>(where: where.ToString()).;

            foreach (var clienteInt in clientesIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(clienteInt);

                    bool clienteNovo = false;

                    var codParc = Convert.ToInt64(clienteInt.CodigoIntegracao);
                    Cliente cliente = _unitOfWork.ClienteRepository.ConsultarPorCodigoIntegracao(codParc);

                    if (cliente == null)
                    {
                        clienteNovo = true;
                        cliente = new Cliente();
                    }

                    cliente.CodigoIntegracao = codParc;
                    cliente.Ativo = clienteInt.Ativo == "S" ? true : false;
                    cliente.NomeFantasia = clienteInt.NomeFantasia;
                    cliente.CNPJCPF = clienteInt.CNPJ;
                    cliente.RazaoSocial = clienteInt.RazaoSocial;
                    //cliente.Classificacao = clienteInt.Classificacao;
                    //cliente.IdRepresentante = clienteInt.IdRepresentante;
                    cliente.Classificacao = "Padrão";
                    cliente.IdRepresentante = 1;

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", cliente.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", "0");

                    if (clienteNovo)
                    {
                        _unitOfWork.ClienteRepository.Add(cliente);
                    }
                    else
                    {
                        _unitOfWork.ClienteRepository.Update(cliente);
                    }

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_unitOfWork);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro na integração do Cliente: {0}.", clienteInt.CodigoIntegracao));

                    continue;
                }
            }
        }
    }
}
