using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.Expedicao;
using FWLog.Services.Model.IntegracaoSankhya;
using FWLog.Services.Model.Transportadora;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class TransportadoraService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public TransportadoraService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task ConsultarTransportadora(bool somenteNovos)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = new StringBuilder();
            where.Append("WHERE TRANSPORTADORA = 'S' ");

            if (somenteNovos)
            {
                where.Append("AND AD_INTEGRARFWLOG = '1' ");
            }
            else
            {
                where.Append("AND AD_INTEGRARFWLOG = '0' ");
            }

            int quantidadeChamadas = 0;

            var transportadoraContadorIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<TransportadoraContadorIntegracao>(where: where.ToString());

            if (transportadoraContadorIntegracao != null)
            {
                try
                {
                    decimal contadorRegistros = Convert.ToInt32(transportadoraContadorIntegracao[0].Quantidade);

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
                    _log.Error("Erro na integração de Transportadoras", ex);
                    return;
                }
            }

            int offsetRows = 0;
            var transportadorasIntegracao = new List<TransportadoraIntegracao>();

            for (int i = 0; i < quantidadeChamadas; i++)
            {
                where = new StringBuilder();
                where.Append("WHERE TRANSPORTADORA = 'S' ");

                if (somenteNovos)
                {
                    where.Append("AND AD_INTEGRARFWLOG = '1' ");
                }
                else
                {
                    where.Append("AND AD_INTEGRARFWLOG = '0' ");
                }

                where.Append("ORDER BY TGFPAR.CODPARC ASC OFFSET " + offsetRows + " ROWS FETCH NEXT 4999 ROWS ONLY ");

                transportadorasIntegracao.AddRange(await IntegracaoSankhya.Instance.PreExecutarQuery<TransportadoraIntegracao>(where: where.ToString()));

                offsetRows += 4999;
            }

            foreach (var transpInt in transportadorasIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(transpInt);

                    bool transportadoraNova = false;

                    var codParc = Convert.ToInt64(transpInt.CodigoIntegracao);
                    Transportadora transportadora = _uow.TransportadoraRepository.ConsultarPorCodigoIntegracao(codParc);

                    if (transportadora == null)
                    {
                        transportadoraNova = true;
                        transportadora = new Transportadora();
                    }

                    transportadora.CodigoIntegracao = codParc;
                    transportadora.Ativo = transpInt.Ativo == "S" ? true : false;
                    transportadora.CNPJ = transpInt.CNPJ;
                    transportadora.RazaoSocial = transpInt.RazaoSocial;
                    transportadora.NomeFantasia = transpInt.NomeFantasia;
                    transportadora.CodigoTransportadora = transpInt.CodigoTransportadora;

                    if (transportadoraNova)
                    {
                        _uow.TransportadoraRepository.Add(transportadora);
                    }
                    else
                    {
                        _uow.TransportadoraRepository.Update(transportadora);
                    }

                    _uow.SaveChanges();

                    if (somenteNovos)
                    {
                        var campoChave = new Dictionary<string, string> { { "CODPARC", transportadora.CodigoIntegracao.ToString() } };
                        await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Parceiro", campoChave, "AD_INTEGRARFWLOG", "0");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração da Transportadora: {0}.", transpInt.CodigoIntegracao), ex);
                }
            }
        }

        public ConsultaTransportadoraResposta ConsultarTransportadora(string codigoOuIdTransportadora)
        {
            Transportadora transportadora;

            if (long.TryParse(codigoOuIdTransportadora, out long idTransportadora))
            {
                transportadora = _uow.TransportadoraRepository.GetById(idTransportadora);
            }
            else
            {
                transportadora = _uow.TransportadoraRepository.ConsultarPorCodigoTransportadora(codigoOuIdTransportadora);
            }

            if (transportadora != null)
            {
                return new ConsultaTransportadoraResposta
                {
                    IdTransportadora = transportadora.IdTransportadora,
                    Nome = transportadora.NomeFantasia,
                    CodigoTransportadora = transportadora.CodigoTransportadora
                };
            }

            return null;
        }

        public EnderecosTransportadoraResposta BuscaEnderecosPorTransportadora(long idTransportadora, long idEmpresa)
        {
            var transportadora = ValidarERetornarTransportadora(idTransportadora);

            var enderecos = _uow.TransportadoraEnderecoRepository.ObterPorIdTransportadoraEmpresa(idTransportadora, idEmpresa);
            
            if (enderecos.NullOrEmpty())
            {
                throw new BusinessException("Transportadora não contém nenhum endereço de armazenagem");
            }

            return new EnderecosTransportadoraResposta()
            {
                IdTransportadora = transportadora.IdTransportadora,
                NomeTransportadora = transportadora.NomeFantasia,
                ListaEnderecos = enderecos.Select(enderecoInstalado => new EnderecosTransportadoraVolumeResposta
                {
                    IdPedidoVendaVolume = 0,
                    CodigoEndereco = enderecoInstalado.EnderecoArmazenagem.Codigo,
                    IdEnderecoArmazenagem = enderecoInstalado.IdEnderecoArmazenagem
                }).ToList()
            };

        }

        public Transportadora ValidarERetornarTransportadora(long idTransportadora)
        {
            if (idTransportadora <= 0)
            {
                throw new BusinessException("Informar a tranportadora.");
            }

            var transportadora = _uow.TransportadoraRepository.GetById(idTransportadora);

            if (transportadora == null)
            {
                throw new BusinessException("Transportadora não encontrada.");
            }

            if (!transportadora.Ativo)
            {
                throw new BusinessException("Transportadora não está ativa.");
            }

            return transportadora;
        }
    }
}