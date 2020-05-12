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

        public async Task Salvar(long idPedidoVenda, long idPedidoVendaVolume, PedidoItemViewModel pedidoItem)
        {
            try
            {
                _uow.PedidoVendaProdutoRepository.Add(new PedidoVendaProduto()
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
                    DataHoraFimSeparacao = null
                });

                await _uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao salvar produto do pedido de venda {0}.", idPedidoVenda), ex);
            }
        }
    }
}
