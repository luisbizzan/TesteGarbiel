using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class PedidoService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public PedidoService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task ConsultarPedidoIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var topSankhya = ConfigurationManager.AppSettings["IntegracaoSankhya_TOP_PedidoVenda"];

            var topSankhyaRequisicao = ConfigurationManager.AppSettings["IntegracaoSankhya_TOP_PedidoVenda_Requisicao"];

            if (string.IsNullOrEmpty(topSankhya))
            {
                throw new BusinessException("As tops de pedido venda não estão configuradas.");
            }

            var where = $" WHERE TGFCAB.TIPMOV = 'P' AND TGFCAB.STATUSNOTA = 'L' AND (TGFCAB.AD_STATUSSEP = 0 OR TGFCAB.AD_STATUSSEP IS NULL) ";
            var inner = " INNER JOIN TGFITE ON TGFCAB.NUNOTA = TGFITE.NUNOTA INNER JOIN TGFTPV ON TGFTPV.CODTIPVENDA = TGFCAB.CODTIPVENDA AND TGFTPV.DHALTER = TGFCAB.DHTIPVENDA";

            List<PedidoIntegracao> pedidosIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<PedidoIntegracao>(where, inner);

            Dictionary<string, List<PedidoIntegracao>> pedidosIntegracaoGrp = pedidosIntegracao.GroupBy(g => g.CodigoIntegracao).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var pedidoIntegracao in pedidosIntegracaoGrp)
            {
                try
                {
                    var pedidoCabecalho = pedidoIntegracao.Value.First();

                    ValidarDadosIntegracao(pedidoCabecalho);

                    if (!topSankhya.Split(',').ToList().Contains(pedidoCabecalho.TopSankhya))
                    {
                        throw new BusinessException(string.Format("Top inválida (CODTIPOPER: {0}). Configuração atual (TOPs: {1}).", pedidoCabecalho.TopSankhya, topSankhya));
                    }

                    var codEmp = Convert.ToInt32(pedidoCabecalho.CodigoIntegracaoEmpresa);
                    Empresa empresa = _uow.EmpresaRepository.ConsultaPorCodigoIntegracao(codEmp);
                    if (empresa == null)
                    {
                        throw new BusinessException(string.Format("Código da Empresa (CODEMP: {0}) inválido", pedidoCabecalho.CodigoIntegracaoEmpresa));
                    }

                    var codParcTransp = Convert.ToInt32(pedidoCabecalho.CodigoIntegracaoTransportadora);
                    Transportadora transportadora = _uow.TransportadoraRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codParcTransp);
                    if (transportadora == null)
                    {
                        throw new BusinessException(string.Format("Código da Transportadora (CODPARCTRANSP: {0}) inválido", pedidoCabecalho.CodigoIntegracaoTransportadora));
                    }

                    var codCliente = Convert.ToInt32(pedidoCabecalho.CodigoIntegracaoCliente);
                    Cliente cliente = _uow.ClienteRepository.ConsultarPorCodigoIntegracao(codCliente);
                    if (cliente == null)
                    {
                        throw new BusinessException(string.Format("Código do Cliente (CODPARC: {0}) inválido", pedidoCabecalho.CodigoIntegracaoCliente));
                    }

                    Representante representante = _uow.RepresentanteRepository.BuscarPorCodigoIntegracaoVendedor(pedidoCabecalho.CodigoIntegracaoRepresentante);
                    if (representante == null)
                    {
                        throw new BusinessException(string.Format("Código do Representante (CODVEND: {0}) inválido", pedidoCabecalho.CodigoIntegracaoRepresentante));
                    }

                    var numeroPedido = Convert.ToInt32(pedidoCabecalho.NroPedidoVenda);
                    if (numeroPedido == 0)
                    {
                        throw new BusinessException($"Código do Pedido (NUMNOTA: {pedidoCabecalho.NroPedidoVenda}) inválido");
                    }

                    bool pedidoNovo = true;

                    var codPedido = Convert.ToInt32(pedidoCabecalho.CodigoIntegracao);
                    Pedido pedido = _uow.PedidoRepository.ObterPorCodigoIntegracao(codPedido);

                    if (pedido != null)
                    {
                        pedidoNovo = false;
                    }
                    else
                    {
                        pedido = new Pedido();
                    }

                    if (!string.IsNullOrEmpty(topSankhyaRequisicao) && topSankhyaRequisicao.Split(',').ToList().Contains(pedidoCabecalho.TopSankhya))
                    {
                        pedido.IsRequisicao = true;
                    }

                    pedido.NroPedido = numeroPedido;
                    pedido.CodigoIntegracao = codPedido;
                    pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoIntegracao;
                    pedido.IdCliente = cliente.IdCliente;
                    pedido.DataCriacao = pedidoCabecalho.DataCriacao == null ? DateTime.Now : DateTime.ParseExact(pedidoCabecalho.DataCriacao, "ddMMyyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    pedido.IdEmpresa = empresa.IdEmpresa;
                    pedido.IdTransportadora = transportadora.IdTransportadora;
                    pedido.IdRepresentante = representante.IdRepresentante;

                    if (!int.TryParse(pedidoCabecalho.TipoPagamentoCodigo, out int tipoPagamentoCodigo))
                    {
                        throw new BusinessException(string.Format("Código do Tipo Pagamento (TGFTPV.CODTIPVENDA: {0}) inválido", pedidoCabecalho.TipoPagamentoCodigo));
                    }

                    pedido.PagamentoCodigoIntegracao = tipoPagamentoCodigo;
                    pedido.PagamentoDescricaoIntegracao = pedidoCabecalho.TipoPagamentoDescricao.TrimOrNull();
                    pedido.PagamentoIsDebitoIntegracao = pedidoCabecalho.TipoPagamentoCartaoDebito == "S";
                    pedido.PagamentoIsCreditoIntegracao = pedidoCabecalho.TipoPagamentoCartaoCredito == "S";
                    pedido.PagamentoIsDinheiroIntegracao = pedidoCabecalho.TipoPagamentoCartaoDinheiro == "S";

                    var itens = pedidoIntegracao.Value.Select(s => new { s.CodigoIntegracao, s.CodigoIntegracaoProduto, s.QtdPedido, s.Sequencia }).ToList();

                    var produtosComQtdePedidoZerado = itens.Where(x => x.QtdPedido == "0" || x.QtdPedido.NullOrEmpty()).Select(x => x.CodigoIntegracaoProduto).ToArray();

                    if (produtosComQtdePedidoZerado.Any() && produtosComQtdePedidoZerado != null)
                    {
                        var produtosConcatenados = string.Join(",", produtosComQtdePedidoZerado);

                        throw new BusinessException(string.Format("Pedido inválido com produtos com QtdePedido zerado (CODPROD: {0})", produtosConcatenados));
                    }

                    var pedidosItens = new List<PedidoItem>();

                    foreach (var item in itens)
                    {
                        var codProduto = Convert.ToInt64(item.CodigoIntegracaoProduto);
                        var qtdPedido = Convert.ToInt32(item.QtdPedido);
                        var sequencia = Convert.ToInt32(item.Sequencia);

                        Produto produto = _uow.ProdutoRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao == codProduto);
                        if (produto == null)
                        {
                            throw new BusinessException(string.Format("Código da Produto (CODPROD: {0}) inválido", item.CodigoIntegracaoProduto));
                        }

                        var pedidoItem = new PedidoItem();

                        pedidoItem.IdProduto = produto.IdProduto;
                        pedidoItem.QtdPedido = qtdPedido;
                        pedidoItem.Sequencia = sequencia;

                        pedidosItens.Add(pedidoItem);
                    }

                    using (var transacao = _uow.CreateTransactionScope())
                    {
                        if (!pedido.PedidoItens.NullOrEmpty())
                        {
                            _uow.PedidoItemRepository.DeleteRange(pedido.PedidoItens.ToList());
                        }

                        pedido.PedidoItens = pedidosItens;

                        if (pedidoNovo)
                        {
                            _uow.PedidoRepository.Add(pedido);
                        }
                        else
                        {
                            _uow.PedidoRepository.Update(pedido);
                        }

                        _uow.SaveChanges();

                        transacao.Complete();
                    }

                    pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.PendenteSeparacao;

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedido.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSSEP", PedidoVendaStatusEnum.PendenteSeparacao.GetHashCode());

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração do pedido: {0}.", pedidoIntegracao.Key), ex);
                }
            }
        }

        public async Task AtualizarStatusPedido(Pedido pedido, PedidoVendaStatusEnum statusPedidoVenda)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            try
            {
                Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedido.CodigoIntegracao.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSSEP", statusPedidoVenda.GetHashCode());
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Erro na atualização do pedido de venda: {0}.", pedido.CodigoIntegracao);
                _log.Error(errorMessage, ex);
                throw new BusinessException(errorMessage);
            }
        }

        public async Task AtualizarStatus(long idPedido, PedidoVendaStatusEnum status)
        {
            try
            {
                var pedido = _uow.PedidoRepository.GetById(idPedido);

                pedido.IdPedidoVendaStatus = status;
                _uow.PedidoRepository.Update(pedido);

                await _uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao atualizar o status do pedido {0}.", idPedido), ex);
            }
        }

        public async Task<int?> ConsultarPedidosPendentesSankhya(long idEmpresa)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                var empresa = _uow.EmpresaRepository.GetById(idEmpresa);

                if (empresa != null)
                {
                    try
                    {
                        var where = $"WHERE TGFCAB.CODEMP = {empresa.CodigoIntegracao} AND TGFCAB.TIPMOV = 'P' AND TGFCAB.STATUSNOTA = 'L' AND (TGFCAB.AD_STATUSSEP = 0 OR TGFCAB.AD_STATUSSEP IS NULL)";

                        var pedidosIntegracaoPendentes = await IntegracaoSankhya.Instance.PreExecutarQuery<PedidoIntegracaoPendente>(where);

                        return pedidosIntegracaoPendentes.Count;
                    }
                    catch (Exception exception)
                    {
                        _log.Error("Erro consultando pedidos pendentes no Sankhya: {0}.", exception);
                    }
                }
            }

            return null;
        }
    }
}