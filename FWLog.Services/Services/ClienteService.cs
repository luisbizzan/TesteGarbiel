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
    public class ClienteService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;
        private ILog _log;

        public ClienteService(UnitOfWork unitOfWork, ILog log)
        {
            _unitOfWork = unitOfWork;
            _log = log;
        }

        public async Task ConsultarCliente()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");
            where.Append("TGFPAR.CGC_CPF IS NOT NULL ");
            where.Append("AND TGFPAR.RAZAOSOCIAL IS NOT NULL ");
            where.Append("AND TGFPAR.CLIENTE = 'S' ");
            where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");

            List<ClienteIntegracao> clientesIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ClienteIntegracao>(where: where.ToString());

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
                    cliente.Classificacao = clienteInt.Classificacao;
                    cliente.Classificacao = "Padrão";
                    cliente.IdRepresentanteInterno = clienteInt.IdRepresentanteInterno == "0" || clienteInt.IdRepresentanteInterno == null
                                                         ? (long?)null
                                                         : _unitOfWork.RepresentanteRepository.BuscarCodigoPeloCodigoIntegracaoVendedor(Convert.ToInt64(clienteInt.IdRepresentanteInterno));
                    cliente.IdRepresentanteExterno = clienteInt.IdRepresentanteExterno == "0" || clienteInt.IdRepresentanteExterno == null
                                                        ? (long?)null
                                                        : _unitOfWork.RepresentanteRepository.BuscarCodigoPeloCodigoIntegracaoVendedor(Convert.ToInt64(clienteInt.IdRepresentanteExterno));

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", cliente.CodigoIntegracao.ToString() } };

                    if (clienteNovo)
                    {
                        _unitOfWork.ClienteRepository.Add(cliente);
                    }
                    else
                    {
                        _unitOfWork.ClienteRepository.Update(cliente);
                    }

                    _unitOfWork.SaveChanges();

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", "0");
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração do Cliente: {0}.", clienteInt.CodigoIntegracao), ex);

                    continue;
                }
            }
        }
    }
}
