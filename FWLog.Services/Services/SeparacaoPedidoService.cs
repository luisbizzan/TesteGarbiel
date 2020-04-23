using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Model.Armazenagem;
using FWLog.Services.Model.Coletor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class SeparacaoPedidoService
    {
        private readonly UnitOfWork _unitOfWork;

        public SeparacaoPedidoService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<long> ConsultaPedidoVendaEmSeparacao(string idUsuario, long idEmpresa)
        {
            var ids = _unitOfWork.PedidoVendaRepository.PesquisarIdsEmSeparacao(idUsuario, idEmpresa);
            return ids;
        }

        //public BuscarPedidoVendaResposta BuscarPedidoVenda(long? idPedidoVenda, string codigoDeBarras, string idUsuario, long idEmpresa)
        //{
        //    ValidarPedidoVenda(idPedidoVenda, codigoDeBarras, idEmpresa);

        //    var pedidoVenda = ConsultaPedidoVenda(idPedidoVenda, codigoDeBarras, idEmpresa);

        //    ValidarPedidoVendaPorUsuario(idUsuario, idEmpresa, pedidoVenda);

        //    var listIdProduto = pedidoVenda.PedidoVendaProdutos.Select(x => x.IdProduto).ToList();

        //    var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.BuscarProdutoEstoquePorIdProduto(idEmpresa, listIdProduto);

        //    return new BuscarPedidoVendaResposta();
        //}

        //public void ValidarPedidoVenda(long? idPedidoVenda, string codigoDeBarras, long idEmpresa)
        //{
        //    var pedidoVenda = ConsultaPedidoVenda(idPedidoVenda, codigoDeBarras, idEmpresa);

        //    if (pedidoVenda == null)
        //    {
        //        throw new BusinessException("O pedido não foi encontrado.");
        //    }

        //    if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ConcluidaComSucesso)
        //    {
        //        throw new BusinessException("O pedido informado já foi separado.");
        //    }

        //    if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao)
        //    {
        //        throw new BusinessException("O pedido informado ainda não está liberado para a separação.");
        //    }

        //    if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteCancelamento
        //        || pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.Cancelado)
        //    {
        //        throw new BusinessException("O pedido informado teve a separação cancelada.");
        //    }
        //}

        //public void ValidarPedidoVendaPorUsuario(string idUsuario, long idEmpresa, PedidoVenda pedidoVenda)
        //{
        //    var pedidoVendaPorUsuario = _unitOfWork.PedidoVendaRepository.ObterPorIdUsuarioEIdEmpresa(idUsuario, idEmpresa);

        //    if (pedidoVendaPorUsuario.Any(x => x.PedidoVendaStatus.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao && x.IdPedidoVenda == pedidoVenda.IdPedidoVenda))
        //    {
        //        throw new BusinessException("Existe um pedido em separação pelo usuário logado que não foi concluído.");
        //    }
        //}

        //public PedidoVenda ConsultaPedidoVenda(long? idPedidoVenda, string codigoDeBarras, long idEmpresa)
        //{
        //    PedidoVenda pedidoVenda;

        //    if (idPedidoVenda != null)
        //    {
        //        pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(idPedidoVenda.Value);
        //    }
        //    else
        //    {
        //        var volume = codigoDeBarras.Substring(codigoDeBarras.Count(), -3);

        //        var nroPedido = codigoDeBarras.Remove(codigoDeBarras.Count(), -6);

        //        pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoENroVolume(int.Parse(nroPedido), int.Parse(volume), idEmpresa);
        //    }

        //    return pedidoVenda;
        //}
    }
}