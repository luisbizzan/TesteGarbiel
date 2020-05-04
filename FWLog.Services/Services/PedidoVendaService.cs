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

        public async Task<int> Salvar(Pedido pedido)
        {
            int idPedidoVenda = 0;

            try
            {
                _uow.PedidoVendaRepository.Add(new PedidoVenda()
                {
                    IdPedido = pedido.IdPedido,
                    DataCriacao = DateTime.Now,
                    IdCliente = pedido.IdCliente,
                    IdEmpresa = pedido.IdEmpresa,
                    IdPedidoVendaStatus = PedidoVendaStatusEnum.PendenteSeparacao,
                    IdRepresentante = pedido.IdRepresentante,
                    IdTransportadora = pedido.IdTransportadora,
                    NroPedidoVenda = pedido.NroPedido,
                    NroVolumes = 0 //Inicialmente salva com 0. Posteriormente, o valor é atualizado.
                });

                idPedidoVenda = await _uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Erro ao salvar o pedido de venda do pedido {0}.", pedido.IdPedido), ex);
            }

            return idPedidoVenda;
        }
    }
}