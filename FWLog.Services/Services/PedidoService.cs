using DartDigital.Library.Exceptions;
using FWLog.Data;
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
    public class PedidoService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public PedidoService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task ConsultaPedido()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = " WHERE TGFCAB.TIPMOV = 'P' AND TGFCAB.STATUSNOTA = 'L' AND (TGFCAB.AD_STATUSSEP = 0 OR TGFCAB.AD_STATUSSEP IS NULL) and TGFCAB.nunota = 703";
            var inner = " INNER JOIN TGFITE ON TGFCAB.NUNOTA = TGFITE.NUNOTA";

            List<PedidoIntegracao> pedidosIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<PedidoIntegracao>(where, inner);

            Dictionary<string, List<PedidoIntegracao>> pedidosIntegracaoGrp = pedidosIntegracao.GroupBy(g => g.CodigoIntegracao).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var pedidoIntegracao in pedidosIntegracaoGrp)
            {
                try
                {
                    var pedidoCabecalho = pedidoIntegracao.Value.First();

                    ValidarDadosIntegracao(pedidoCabecalho);

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

                    var codRep = Convert.ToInt32(pedidoCabecalho.CodigoIntegracaoRepresentante);
                    Representante representante = _uow.RepresentanteRepository.ConsultarPorCodigoIntegracao(codRep);
                    if (representante == null)
                    {
                        throw new BusinessException(string.Format("Código do Representante (CODVEND: {0}) inválido", pedidoCabecalho.CodigoIntegracaoRepresentante));
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

                    pedido.NroPedido = Convert.ToInt32(pedidoCabecalho.NroPedidoVenda);
                    pedido.CodigoIntegracao = codPedido;
                    pedido.IdPedidoStatus = PedidoStatusEnum.ProcessandoIntegracao;
                    pedido.IdCliente = cliente.IdCliente;
                    pedido.DataCriacao = pedidoCabecalho.DataCriacao == null ? DateTime.Now : DateTime.ParseExact(pedidoCabecalho.DataCriacao, "ddMMyyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    pedido.IdEmpresa = empresa.IdEmpresa;
                    pedido.IdTransportadora = transportadora.IdTransportadora;
                    pedido.IdRepresentante = representante.IdRepresentante;

                    var itens = pedidoIntegracao.Value.Select(s => new { s.CodigoIntegracao, s.CodigoIntegracaoProduto, s.QtdPedido, s.Sequencia }).ToList();

                    List<PedidoItem> pedidosItens = new List<PedidoItem>();

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

                        bool itemNovo = false;

                        //Delete dos itenss
                        PedidoItem pedidoItem = pedido.PedidoItens.FirstOrDefault(f => f.IdProduto == produto.IdProduto && f.Sequencia == sequencia);

                        if (pedidoItem == null)
                        {
                            pedidoItem = new PedidoItem();
                            itemNovo = true;
                        }

                        pedidoItem.IdProduto = produto.IdProduto;
                        pedidoItem.QtdPedido = qtdPedido;
                        pedidoItem.Sequencia = sequencia;

                        if (itemNovo)
                        {
                            pedidosItens.Add(pedidoItem);

                            pedido.PedidoItens = pedidosItens;
                        }
                    }

                    if (pedidoNovo)
                    {
                        _uow.PedidoRepository.Add(pedido);
                    }

                    _uow.SaveChanges();

                    pedido.IdPedidoStatus = PedidoStatusEnum.ProcessandoIntegracao;

                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedido.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSSEP", PedidoStatusEnum.Integrado.GetHashCode());

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração do pedido: {0}.", pedidoIntegracao.Key), ex);
                }
            }
        }
    }
}


