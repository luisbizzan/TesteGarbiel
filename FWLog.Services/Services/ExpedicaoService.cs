using DartDigital.Library.Exceptions;
using FWLog.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void ValidarReferenciaPedido(string referenciaPedido, long idEmpresa)
        {
            if (referenciaPedido.NullOrEmpty())
            {
                throw new BusinessException("Código de barras do pedido deve ser infomado.");
            }

            if (referenciaPedido.Length < 7)
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroPedidoString = referenciaPedido.Substring(0, referenciaPedido.Length - 6);

            if (!int.TryParse(numeroPedidoString, out int numeroPedido))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroVolumeString = referenciaPedido.Substring(referenciaPedido.Length - 3);

            if (!int.TryParse(numeroVolumeString, out int numeroVolume))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(numeroPedido, idEmpresa);

            if (pedidoVenda == null)
            {
                throw new BusinessException("Pedido não encontrado.");
            }

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.FirstOrDefault(volume => volume.NroVolume == numeroVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("Volume fornecido não encontrado.");
            }

            if (pedidoVendaVolume.IdEnderecoArmazTransportadora != null)
            {
                throw new BusinessException($"Volume já instalado em: {pedidoVendaVolume.EnderecoTransportadora.Codigo}");
            }

            var codigoTransportadora = referenciaPedido.Substring(referenciaPedido.Length - 6).Replace(numeroVolumeString, "");

            if (pedidoVenda.Transportadora.CodigoTransportadora != codigoTransportadora)
            {
                throw new BusinessException("Transportadora da referência está incorreta");
            }

            if (pedidoVenda.Transportadora.Enderecos.NullOrEmpty())
            {
                throw new BusinessException("Endereço da transportadora não cadastrado!");
            }
        }
    }
}
