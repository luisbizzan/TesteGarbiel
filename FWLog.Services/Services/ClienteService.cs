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
using DartDigital.Library.Exceptions;

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

        public async Task ConsultarCliente(bool somenteNovos = true)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = new StringBuilder();
            where.Append("WHERE TGFPAR.CLIENTE = 'S' ");

            if (somenteNovos)
            {
                where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");
            }

            int quantidadeChamadas = 0;

            List<ClienteContadorIntegracao> clienteContadorIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ClienteContadorIntegracao>(where: where.ToString());

            if (clienteContadorIntegracao != null)
            {
                try
                {
                    decimal contadorRegistros = Convert.ToInt32(clienteContadorIntegracao[0].Quantidade);
                    
                    if(contadorRegistros < 4999)
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
                    _log.Error("Erro na integração de Cliente", ex);
                    return;
                }
            }

            int offsetRows = 0;
            var join = new StringBuilder();
            join.Append("LEFT JOIN TGFCPL ON TGFPAR.CODPARC = TGFCPL.CODPARC ");
            join.Append("LEFT JOIN TSIEND ON TGFCPL.CODENDENTREGA = TSIEND.CODEND ");
            join.Append("LEFT JOIN TSICID ON TGFCPL.CODCIDENTREGA = TSICID.CODCID ");
            join.Append("LEFT JOIN TSIUFS ON TSICID.UF = TSIUFS.CODUF ");

            for (int i = 0; i < quantidadeChamadas; i++)
            {
                where = new StringBuilder();
                where.Append("WHERE TGFPAR.CLIENTE = 'S' ");

                if (somenteNovos)
                {
                    where.Append("AND TGFPAR.AD_INTEGRARFWLOG = '1' ");
                }

                where.Append("ORDER BY TGFPAR.CODPARC ASC OFFSET "+ offsetRows + " ROWS FETCH NEXT 4999 ROWS ONLY ");

                List<ClienteIntegracao> clientesIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ClienteIntegracao>(where: where.ToString(), inner: join.ToString());

                foreach (var clienteInt in clientesIntegracao)
                {
                    try
                    {
                        ValidarDadosIntegracao(clienteInt);

                        if (clienteInt.CodigoIntgracaoEndereco == "0")
                        {
                            throw new BusinessException($"Código do Endereço de Entrega (CODENDENTREGA: 0) inválido.");
                        }

                        if (clienteInt.CodigoIntegracaoCidade == "0")
                        {
                            throw new BusinessException($"Código da Cidade de Entrega (CODCIDENTREGA: 0) inválido.");
                        }

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
                        cliente.IdRepresentanteInterno = _unitOfWork.RepresentanteRepository.BuscarCodigoPeloCodigoIntegracaoVendedor(clienteInt.IdRepresentanteInterno);
                        cliente.IdRepresentanteExterno = _unitOfWork.RepresentanteRepository.BuscarCodigoPeloCodigoIntegracaoVendedor(clienteInt.IdRepresentanteExterno);
                        cliente.CEP = clienteInt.CEP;
                        cliente.Telefone = clienteInt.Telefone;
                        cliente.UF = clienteInt.UF;
                        cliente.Endereco = clienteInt.Endereco;
                        cliente.Numero = clienteInt.Numero;
                        cliente.Cidade = clienteInt.Cidade;

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

                i++;
                offsetRows += 4999;
            }
        }

        public async Task LimparIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder inner = new StringBuilder();
            inner.Append("INNER JOIN TGFCPL ON TGFPAR.CODPARC = TGFCPL.CODPARC ");
            inner.Append("INNER JOIN TSIEND ON TGFCPL.CODENDENTREGA = TSIEND.CODEND ");
            inner.Append("INNER JOIN TSICID ON TGFCPL.CODCIDENTREGA = TSICID.CODCID ");
            inner.Append("INNER JOIN TSIUFS ON TSICID.UF = TSIUFS.CODUF ");

            StringBuilder where = new StringBuilder();
            where.Append("WHERE ");       
            where.Append("AND TGFPAR.CLIENTE = 'S' ");
       
            List<ClienteIntegracao> clientesIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ClienteIntegracao>(where: where.ToString(), inner: inner.ToString());

            foreach (var fornecInt in clientesIntegracao)
            {
                Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPARC", fornecInt.CodigoIntegracao.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", '1');
            }
        }
    }
}
