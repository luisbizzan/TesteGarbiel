using FWLog.Data;
using FWLog.Data.Models;
using log4net;
using System;
using System.Threading.Tasks;

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

        public async Task<long> Salvar(Pedido pedido)
        {
            long idPedidoVenda = 0;

            try
            {
                var pedidoVendaRepository = _uow.PedidoVendaRepository.ObterPorIdPedido(pedido.IdPedido);

                if (pedidoVendaRepository != null)
                {
                    return pedidoVendaRepository.IdPedidoVenda;
                }

                var pedidoVenda = new PedidoVenda()
                {
                    IdPedido = pedido.IdPedido,
                    IdCliente = pedido.IdCliente,
                    IdEmpresa = pedido.IdEmpresa,
                    IdPedidoVendaStatus = PedidoVendaStatusEnum.PendenteSeparacao,
                    IdRepresentante = pedido.IdRepresentante,
                    IdTransportadora = pedido.IdTransportadora,
                    NroPedidoVenda = pedido.NroPedido,
                    NroVolumes = 0 //Inicialmente salva com 0. Posteriormente, o valor é atualizado.
                };

                _uow.PedidoVendaRepository.Add(pedidoVenda);

                await _uow.SaveChangesAsync();

                idPedidoVenda = pedidoVenda.IdPedidoVenda;
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao salvar o pedido de venda do pedido {0}.", pedido.IdPedido), ex);
            }

            return idPedidoVenda;
        }

        public async Task AtualizarQuantidadeVolume(long idPedidoVenda, int quantidade)
        {
            try
            {
                var pedidoVenda = _uow.PedidoVendaRepository.GetById(idPedidoVenda);

                if (pedidoVenda != null)
                    pedidoVenda.NroVolumes = pedidoVenda.NroVolumes + quantidade;

                await _uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao atualizar a quantidade de volumes do pedido de venda {0}.", idPedidoVenda), ex);
            }
        }

        public async Task AtualizarStatus(long idPedidoVenda, PedidoVendaStatusEnum status)
        {
            try
            {
                var pedidoVenda = _uow.PedidoVendaRepository.GetById(idPedidoVenda);

                if (pedidoVenda != null)
                    pedidoVenda.IdPedidoVendaStatus = status;

                await _uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao atualizar o status do pedido de venda {0}.", idPedidoVenda), ex);
            }
        }
    }
}