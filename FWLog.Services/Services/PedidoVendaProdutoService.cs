using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.SeparacaoPedido;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class PedidoVendaProdutoService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public PedidoVendaProdutoService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public PedidoVendaProduto RetornarParaSalvar(long idPedidoVenda, long idPedidoVendaVolume, PedidoItemViewModel pedidoItem)
        {
            PedidoVendaProduto pedidoVendaProduto = new PedidoVendaProduto();

            try
            {
                pedidoVendaProduto = new PedidoVendaProduto()
                {
                    IdPedidoVenda = idPedidoVenda,
                    IdPedidoVendaVolume = idPedidoVendaVolume,
                    IdProduto = pedidoItem.Produto.IdProduto,
                    IdEnderecoArmazenagem = pedidoItem.EnderecoSeparacao.IdEnderecoArmazenagem.Value,
                    IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao,
                    QtdSeparar = pedidoItem.Quantidade,
                    QtdSeparada = null,
                    CubagemProduto = pedidoItem.Produto.CubagemProduto.Value,
                    PesoProduto = pedidoItem.Produto.PesoBruto,
                    DataHoraInicioSeparacao = null,
                    DataHoraFimSeparacao = null,
                    IdLote = pedidoItem.IdLote
                };
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao salvar produto do pedido de venda {0}.", idPedidoVenda), ex);
            }

            return pedidoVendaProduto;
        }
    }
}
