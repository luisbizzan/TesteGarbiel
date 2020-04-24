using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class SeparacaoPedidoService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ColetorHistoricoService _coletorHistoricoService;
        private ILog _log;

        public SeparacaoPedidoService(UnitOfWork unitOfWork, ColetorHistoricoService coletorHistoricoService, ILog log)
        {
            _unitOfWork = unitOfWork;
            _coletorHistoricoService = coletorHistoricoService;
            _log = log;
        }

        public List<long> ConsultaPedidoVendaEmSeparacao(string idUsuario, long idEmpresa)
        {
            var ids = _unitOfWork.PedidoVendaRepository.PesquisarIdsEmSeparacao(idUsuario, idEmpresa);
            return ids;
        }

        public async Task AtualizarStatusPedidoVenda(PedidoVenda pedidoVenda, PedidoVendaStatusEnum statusPedidoVenda)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            try
            {
                Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedidoVenda.CodigoIntegracao.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSSEP", statusPedidoVenda.GetHashCode());

            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Erro na atualização do pedido de venda: {0}.", pedidoVenda.CodigoIntegracao);
                _log.Error(errorMessage, ex);
                throw new BusinessException(errorMessage);
            }
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

        public void CancelarPedidoSeparacao(long idPedidoVenda, string idUsuarioPermissaoCancelamento, string idUsuarioOperacao, long idEmpresa)
        {
            if (idPedidoVenda <= 0)
            {
                throw new BusinessException("O pedido deve ser informado.");
            }
            var pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(idPedidoVenda);

            if (pedidoVenda == null)
            {
                throw new BusinessException("O pedido não foi encontrado.");
            }

            if (pedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("O pedido não pertence a empresa do usuário logado.");

            }

            if (pedidoVenda.IdUsuarioSeparacao != idUsuarioOperacao)
            {
                throw new BusinessException("O pedido não pertence ao usuário logado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("O pedido não está em separação.");
            }

            if (idUsuarioPermissaoCancelamento.NullOrEmpty())
            {
                throw new BusinessException("O usuário da permissão deve ser informado.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao;
                pedidoVenda.IdUsuarioSeparacao = null;
                pedidoVenda.DataHoraInicioSeparacao = null;
                pedidoVenda.DataHoraFimSeparacao = null;

                _unitOfWork.SaveChanges();

                //TODO:
                /*
                    Atualizar a LoteProdutoEndereco abatendo da coluna Quantidade a PedidoVendaProduto.QtdSeparada filtrando o IdProduto, IdEmpresa e IdEnderecoArmazenagem

                    O IdEnderecoArmazenagem, deve ser carregado da PedidoVendaProduto que foi gerado pelo Robô de Separação

                    Atualizar a PedidoVendaProduto.QtdSeparada de todos os produtos do pedido para zero para que a separação seja reiniciada

                    Atualizar as colunas a seguir da PedidoVendaProduto IdUsuarioAutorizacaoZerarPedido, DataHoraInicioSeparacao, DataHoraFimSeparacao para NULL e a StatusSeparacao para “2 - Enviado para Separação”
                */

                //var listaPedidoVendaProduto = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdPedidoVenda(idPedidoVenda);

                //foreach (var pedidoVendaProduto in listaPedidoVendaProduto)
                //{
                //}

                //TODO: Devem ser atualizadas as colunas da PedidoVendaVolume
                //PedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao;
                //PedidoVendaVolume.DataHoraInicioSeparacao = null;
                //PedidoVendaVolume.DataHoraFimSeparacao = null;
                //PedidoVendaVolume.IdCaixaVolume = null;

                //TODO: Adicionar logs de PedidoVendaVolume
                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.CancelamentoSeparacao,
                    Descricao = $"Cancelou a separação do pedido {idPedidoVenda} com permissão do usuário {idUsuarioPermissaoCancelamento}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioOperacao
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);

                //TODO: Atualizar o status da separação no Sankhya

                transacao.Complete();
            }
        }
    }
}