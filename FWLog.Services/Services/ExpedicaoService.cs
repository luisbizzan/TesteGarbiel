using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using log4net;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ExpedicaoService
    {
        private readonly UnitOfWork _unitOfWork;
        private ILog _log;

        public ExpedicaoService(UnitOfWork unitOfWork, ILog log)
        {
            _unitOfWork = unitOfWork;
            _log = log;
        }

        public void IniciarExpedicaoPedidoVenda(long idPedidoVenda, long idPedidoVendaVolume, string idUsuario, long idEmpresa)
        {
            // validações
            if (idPedidoVenda <= 0)
            {
                throw new BusinessException("O pedido deve ser informado.");
            }

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorIdPedidoVendaEIdEmpresa(idPedidoVenda, idEmpresa);
            if (pedidoVenda == null)
            {
                throw new BusinessException("O pedido não foi encontrado.");
            }

            if (pedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("O pedido não pertence a empresa do usuário logado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.ConcluidaComSucesso)
            {
                throw new BusinessException("Separação do volume não está finalizada.");
            }

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.First(f => f.IdPedidoVendaVolume == idPedidoVendaVolume);
            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("O volume não foi encontrado.");
            }

            //Verificar se a NF do pedido foi emitida, e confirmada. Status: “A” no Sankhya e as guias foram pagas, status “Y” no Sankhya; Caso não tenha, exibir mensagem “NF e Guias não estão emitidas/ pagas” (Tabela Pedido)            
            if (pedidoVenda.NroPedidoVenda != default)
            {                
                throw new BusinessException("NF e Guias não estão emitidas/pagas.");
            }

            using (var transaction = _unitOfWork.CreateTransactionScope())
            {
                pedidoVendaVolume.DataHoraInicioSeparacao = DateTime.Now;
                pedidoVendaVolume.IdUsuarioInstalTransportadora = idUsuario;

                _unitOfWork.PedidoVendaVolumeRepository.Update(pedidoVendaVolume);
                _unitOfWork.SaveChanges();
                transaction.Complete();
            }

        }
    }
   
}
