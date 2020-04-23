using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace FWLog.Services.Services
{
    public class PedidoVendaService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public PedidoVendaService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task ConsultaPedidoVenda()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = " WHERE TGFCAB.TIPMOV = 'P' AND TGFCAB.STATUSNOTA = 'L' AND (TGFCAB.AD_STATUSSEP = 0 OR TGFCAB.AD_STATUSSEP IS NULL) and TGFCAB.nunota = 703";
            var inner = " INNER JOIN TGFITE ON TGFCAB.NUNOTA = TGFITE.NUNOTA";

            List<PedidoVendaIntegracao> pedidosVendaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<PedidoVendaIntegracao>(where, inner);

            Dictionary<string, List<PedidoVendaIntegracao>> pedidosVendaIntegracaoGrp = pedidosVendaIntegracao.GroupBy(g => g.CodigoIntegracao).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var pedidoIntegracao in pedidosVendaIntegracaoGrp)
            {
                try
                {
                    var pedidoVendaIntegracao = pedidoIntegracao.Value.First();

                    ValidarDadosIntegracao(pedidoVendaIntegracao);

                    var codEmp = Convert.ToInt32(pedidoVendaIntegracao.CodigoIntegracaoEmpresa);
                    Empresa empresa = _uow.EmpresaRepository.ConsultaPorCodigoIntegracao(codEmp);
                    if (empresa == null)
                    {
                        throw new BusinessException(string.Format("Código da Empresa (CODEMP: {0}) inválido", pedidoVendaIntegracao.CodigoIntegracaoEmpresa));
                    }

                    var codParcTransp = Convert.ToInt32(pedidoVendaIntegracao.CodigoIntegracaoTransportadora);
                    Transportadora transportadora = _uow.TransportadoraRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codParcTransp);
                    if (transportadora == null)
                    {
                        throw new BusinessException(string.Format("Código da Transportadora (CODPARCTRANSP: {0}) inválido", pedidoVendaIntegracao.CodigoIntegracaoTransportadora));
                    }

                    var codCliente = Convert.ToInt32(pedidoVendaIntegracao.CodigoIntegracaoCliente);
                    Cliente cliente = _uow.ClienteRepository.ConsultarPorCodigoIntegracao(codCliente);
                    if (cliente == null)
                    {
                        throw new BusinessException(string.Format("Código do Cliente (CODPARC: {0}) inválido", pedidoVendaIntegracao.CodigoIntegracaoCliente));
                    }

                    var codRep = Convert.ToInt32(pedidoVendaIntegracao.CodigoIntegracaoRepresentante);
                    Representante representante = _uow.RepresentanteRepository.ConsultarPorCodigoIntegracao(codRep);
                    if (representante == null)
                    {
                        throw new BusinessException(string.Format("Código do Representante (CODVEND: {0}) inválido", pedidoVendaIntegracao.CodigoIntegracaoRepresentante));
                    }

                    bool pedidoNovo = true;

                    var codPedido = Convert.ToInt32(pedidoVendaIntegracao.CodigoIntegracao);
                    PedidoVenda pedidoVenda = _uow.PedidoVendaRepository.ObterPorCodigoIntegracao(codPedido);

                    if (pedidoVenda != null)
                    {
                        pedidoNovo = false;
                    }
                    else
                    {
                        pedidoVenda = new PedidoVenda();
                    }

                    pedidoVenda.NroPedidoVenda = Convert.ToInt32(pedidoVendaIntegracao.NroPedidoVenda);
                    pedidoVenda.CodigoIntegracao = codPedido;
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.PendenteSeparacao;
                    pedidoVenda.IdCliente = cliente.IdCliente;
                    pedidoVenda.DataCriacao = pedidoVendaIntegracao.DataCriacao == null ? DateTime.Now : DateTime.ParseExact(pedidoVendaIntegracao.DataCriacao, "ddMMyyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    pedidoVenda.IdEmpresa = empresa.IdEmpresa;
                    pedidoVenda.IdTransportadora = transportadora.IdTransportadora;
                    pedidoVenda.IdRepresentante = representante.IdRepresentante;

                    var pedidoVendaProdutos = pedidoIntegracao.Value.Select(s => new { s.CodigoIntegracao, s.CodigoIntegracaoProduto, s.QtdPedido, s.Sequencia }).ToList();

                    List<PedidoVendaProduto> pedidosVendaProdutos = new List<PedidoVendaProduto>();

                    foreach (var item in pedidoVendaProdutos)
                    {
                        var codProduto = Convert.ToInt64(item.CodigoIntegracaoProduto);
                        var qtdPedido = Convert.ToInt32(item.QtdPedido);
                        var sequencia = Convert.ToInt32(item.Sequencia);

                        Produto produto = _uow.ProdutoRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codProduto);
                        if (produto == null)
                        {
                            throw new BusinessException(string.Format("Código da Produto (CODPROD: {0}) inválido", item.CodigoIntegracaoProduto));
                        }

                        bool itemNovo = false;
                        
                        PedidoVendaProduto pedidoVendaProduto = pedidoVenda.PedidoVendaProdutos.FirstOrDefault(f => f.IdProduto == produto.IdProduto && f.Sequencia == sequencia);

                        if (pedidoVendaProduto == null)
                        {
                            pedidoVendaProduto = new PedidoVendaProduto();
                            itemNovo = true;
                        }

                        pedidoVendaProduto.IdProduto = produto.IdProduto;
                        pedidoVendaProduto.QtdPedido = qtdPedido;
                        pedidoVendaProduto.Sequencia = sequencia;

                        pedidoVendaProduto.IdPedidoVendaProdutoStatus = PedidoVendaProdutoStatusEnum.ProcessandoIntegracao;

                        if (itemNovo)
                        {
                            pedidosVendaProdutos.Add(pedidoVendaProduto);

                            pedidoVenda.PedidoVendaProdutos = pedidosVendaProdutos;
                        }
                    }

                    if (pedidoNovo)
                    {
                        _uow.PedidoVendaRepository.Add(pedidoVenda);
                    }

                    _uow.SaveChanges();

                   // pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.AguardandoSeparacao;

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedidoVenda.CodigoIntegracao.ToString() } };

                   // await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSSEP", PedidoVendaStatusEnum.AguardandoSeparacao.GetHashCode());

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração do pedido de venda: {0}.", pedidoIntegracao.Key), ex);
                }
            }
        }
    }
}


