﻿using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using log4net;
using System;
using System.Collections.Generic;
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

        public async Task AtualizaNotasFiscaisPedidos()
        {
            var pedidosSemNota = _unitOfWork.PedidoRepository.ObterPedidosSemNotaFiscal();

            foreach (var pedido in pedidosSemNota)
            {
                try
                {
                    var where = $"WHERE TGFVAR.NUNOTAORIG = {pedido.CodigoIntegracao} AND TGFCAB.STATUSNFE = 'A'";

                    var inner = "INNER JOIN TGFVAR ON TGFVAR.NUNOTA = TGFCAB.NUNOTA";

                    var dadosIntegracaoSankhya = await IntegracaoSankhya.Instance.PreExecutarQuery<PedidoNumeroNotaFiscalIntegracao>(where: where, inner: inner);

                    var dadosNotaFiscal = dadosIntegracaoSankhya.FirstOrDefault();

                    if (dadosNotaFiscal != null && long.TryParse(dadosNotaFiscal.NumeroNotaFiscal, out long numeroNotaFiscal))
                    {
                        pedido.CodigoIntegracaoNotaFiscal = numeroNotaFiscal;

                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                catch (Exception exception)
                {
                    _log.Error(string.Format("Erro na tentativa de integração de nota fiscal do pedido {0}", pedido.IdPedido), exception);
                }
            }
        }

        public void ValidaEnderecoInstalacaoVolume(long idPedidoVendaVolume, long idEnderecoArmazenagem, long idEmpresa)
        {
            if (idPedidoVendaVolume <= 0)
            {
                throw new BusinessException("O volume deve ser informado.");
            }

            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("O volume não foi encontrado.");
            }

            if (pedidoVendaVolume.PedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("O volume informado não pertence a empresa do usuário logado.");
            }

            var enderecoTransportadora = _unitOfWork.TransportadoraEnderecoRepository.ObterPorEnderecoTransportadoraEmpresa(idEnderecoArmazenagem, pedidoVendaVolume.PedidoVenda.IdTransportadora, idEmpresa);

            if (enderecoTransportadora == null)
            {
                throw new BusinessException("Endereço não pertence a transportadora.");
            }
        }

        public void SalvaInstalacaoVolumes(List<long> listaIdsVolumes, long idEnderecoArmazenagem, long idEmpresa, string idUsuario)
        {
            if (listaIdsVolumes.NullOrEmpty())
            {
                throw new BusinessException("A lista de volumes deve ser informada.");
            }

            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            foreach (var idPedidoVendaVolume in listaIdsVolumes)
            {
                ValidaEnderecoInstalacaoVolume(idPedidoVendaVolume, idEnderecoArmazenagem, idEmpresa);
            }

            var listaPedidoVendaVolume = new List<PedidoVendaVolume>();

            foreach (var idPedidoVendaVolume in listaIdsVolumes)
            {
                var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

                if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.ConcluidaComSucesso)
                {
                    throw new BusinessException($"Volume {pedidoVendaVolume.NroVolume} com status inválido.");
                }

                listaPedidoVendaVolume.Add(pedidoVendaVolume);
            }

            if (listaPedidoVendaVolume.Select(pvv => pvv.IdPedidoVenda).Distinct().Count() > 1)
            {
                throw new BusinessException("Existem volumes de diferentes pedidos.");
            }

            //TODO: Salvar dados
            //using (var transacao = _unitOfWork.CreateTransactionScope())
            //{
            //    transacao.Complete();
            //}
        }
    }
}