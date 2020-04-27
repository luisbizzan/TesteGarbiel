﻿using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Model.SeparacaoPedido;
using log4net;
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

        public async Task AtualizarStatusPedidoVenda(Pedido pedido, PedidoVendaStatusEnum statusPedidoVenda)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            try
            {
                Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedido.CodigoIntegracao.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSSEP", statusPedidoVenda.GetHashCode());

            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Erro na atualização do pedido de venda: {0}.", pedido.CodigoIntegracao);
                _log.Error(errorMessage, ex);
                throw new BusinessException(errorMessage);
            }
        }

        public BuscarPedidoVendaResposta BuscarPedidoVenda(string referenciaPedido, string idUsuario, long idEmpresa)
        {
            var pedidoVenda = ValidarPedidoVenda(referenciaPedido, idEmpresa);

            ValidarPedidoVendaPorUsuario(idUsuario, idEmpresa, pedidoVenda);

            //var listIdProduto = pedidoVenda.PedidoVendaProdutos.Select(x => x.IdProduto).ToList();

            //var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.BuscarProdutoEstoquePorIdProduto(idEmpresa, listIdProduto);

            //TODO: Falta Finalizar o processo de busca das informações e montar o objeto de resposta
            return new BuscarPedidoVendaResposta();
        }

        public PedidoVenda ValidarPedidoVenda(string referenciaPedido, long idEmpresa)
        {
            if (referenciaPedido.NullOrEmpty())
            {
                throw new BusinessException("Referência do pedido deve ser infomada.");
            }

            var pedidoVenda = ConsultaPedidoVenda(referenciaPedido, idEmpresa);

            if (pedidoVenda == null)
            {
                throw new BusinessException("O pedido informado não foi encontrado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ConcluidaComSucesso)
            {
                throw new BusinessException("O pedido informado já foi separado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao)
            {
                throw new BusinessException("O pedido informado ainda não está liberado para a separação.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteCancelamento
                || pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.Cancelado)
            {
                throw new BusinessException("O pedido informado teve a separação cancelada.");
            }

            return pedidoVenda;
        }

        public void ValidarPedidoVendaPorUsuario(string idUsuario, long idEmpresa, PedidoVenda pedidoVenda)
        {
            var pedidoVendaPorUsuario = _unitOfWork.PedidoVendaRepository.ObterPorIdUsuarioEIdEmpresa(idUsuario, idEmpresa);

            if (pedidoVendaPorUsuario.Any(x => x.PedidoVendaStatus.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao && x.IdPedidoVenda != pedidoVenda.IdPedidoVenda))
            {
                throw new BusinessException("Existe um pedido em separação pelo usuário logado que não foi concluído.");
            }
        }

        //TODO: Falta definir os status e adicionar IdPontoArmazenagemSeparacao na UsuarioEmpresa
        //public void ValidarPedidoVendaVolumePorUsuario(string idUsuario, long idEmpresa, List<ProdutoEstoque> produtoEstoque)
        //{
        //    var usuarioEmpresa = _unitOfWork.UsuarioEmpresaRepository.Obter(idEmpresa, idUsuario);
        //    var range = Enumerable.Range(usuarioEmpresa.CorredorEstoqueInicio.Value, usuarioEmpresa.CorredorSeparacaoFim.Value);

        //    foreach (var item in produtoEstoque)
        //    {
        //        if (!range.Contains(item.EnderecoArmazenagem.Corredor))
        //    }
        //}

        public PedidoVenda ConsultaPedidoVenda(string referenciaPedido, long idEmpresa)
        {
            PedidoVenda pedidoVenda;

            if (long.TryParse(referenciaPedido, out long idPedidoVenda))
            {
                pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(idPedidoVenda);
            }
            else
            {
                var volume = referenciaPedido.Substring(referenciaPedido.Count(), -3);

                var nroPedido = referenciaPedido.Remove(referenciaPedido.Count(), -6);

                pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoENroVolume(int.Parse(nroPedido), int.Parse(volume), idEmpresa);
            }

            return pedidoVenda;
        }

        private async Task AjustarQuantidadeVolume(long idEnderecoArmazenagem, long idProduto, int quantidadeAdicionar, long idEmpresa, string idUsuarioAjuste)
        {
            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoProdutoEmpresa(idEnderecoArmazenagem, idProduto, idEmpresa);

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            var quantidadeAnterior = loteProdutoEndereco.Quantidade;

            var novaQuantidade = loteProdutoEndereco.Quantidade += quantidadeAdicionar;

            var pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * novaQuantidade;

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                loteProdutoEndereco.Quantidade = novaQuantidade;
                loteProdutoEndereco.PesoTotal = pesoInstalacao;

                _unitOfWork.LoteProdutoEnderecoRepository.Update(loteProdutoEndereco);
                await _unitOfWork.SaveChangesAsync();

                if (loteProdutoEndereco.IdLote.HasValue)
                {
                    var loteMovimentacao = new LoteMovimentacao
                    {
                        IdEmpresa = idEmpresa,
                        IdLote = loteProdutoEndereco.IdLote.Value,
                        IdProduto = loteProdutoEndereco.IdProduto,
                        IdEnderecoArmazenagem = loteProdutoEndereco.IdEnderecoArmazenagem,
                        IdUsuarioMovimentacao = idUsuarioAjuste,
                        Quantidade = loteProdutoEndereco.Quantidade,
                        IdLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Ajuste,
                        DataHora = DateTime.Now
                    };

                    _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                    await _unitOfWork.SaveChangesAsync();
                }

                transacao.Complete();

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.AjustarQuantidade,
                    Descricao = $"Ajustou a quantidade de {quantidadeAnterior} para {loteProdutoEndereco.Quantidade} unidade(s) do produto {loteProdutoEndereco.Produto.Referencia} do lote {loteProdutoEndereco.Lote.IdLote} do endereço {loteProdutoEndereco.EnderecoArmazenagem.Codigo}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioAjuste
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
            }
        }

        public async Task CancelarPedidoSeparacao(long idPedidoVenda, string usuarioPermissaoCancelamento, string idUsuarioOperacao, long idEmpresa)
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

            if (usuarioPermissaoCancelamento.NullOrEmpty())
            {
                throw new BusinessException("O usuário da permissão deve ser informado.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var novoStatusSeparacao = PedidoVendaStatusEnum.EnviadoSeparacao;

                pedidoVenda.IdPedidoVendaStatus = novoStatusSeparacao;
                pedidoVenda.IdUsuarioSeparacao = null;
                pedidoVenda.DataHoraInicioSeparacao = null;
                pedidoVenda.DataHoraFimSeparacao = null;

                _unitOfWork.SaveChanges();

                foreach (var pedidoVendaProduto in pedidoVenda.PedidoVendaProdutos.ToList())
                {
                    if (pedidoVendaProduto.QtdSeparada.HasValue)
                    {
                        await AjustarQuantidadeVolume(pedidoVendaProduto.IdEnderecoArmazenagem, pedidoVendaProduto.IdProduto, pedidoVendaProduto.QtdSeparada.Value, idEmpresa, idUsuarioOperacao);
                    }

                    pedidoVendaProduto.IdPedidoVendaProdutoStatus = PedidoVendaProdutoStatusEnum.AguardandoSeparacao;
                    pedidoVendaProduto.DataHoraInicioSeparacao = null;
                    pedidoVendaProduto.DataHoraFimSeparacao = null;
                    pedidoVendaProduto.IdUsuarioAutorizacaoZerar = idUsuarioOperacao;
                    pedidoVendaProduto.QtdSeparada = 0;
                }

                _unitOfWork.SaveChanges();

                foreach (var pedidoVendaVolume in pedidoVenda.PedidoVendaVolumes.ToList())
                {
                    pedidoVendaVolume.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVendaVolume.DataHoraInicioSeparacao = null;
                    pedidoVendaVolume.DataHoraFimSeparacao = null;
                    pedidoVendaVolume.IdCaixaVolume = null;
                }

                _unitOfWork.SaveChanges();

                await AtualizarStatusPedidoVenda(pedidoVenda.Pedido, novoStatusSeparacao);

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.CancelamentoSeparacao,
                    Descricao = $"Cancelou a separação do pedido {idPedidoVenda} com permissão do usuário {usuarioPermissaoCancelamento}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioOperacao
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);

                transacao.Complete();
            }
        }

        public async Task IniciarSeparacaoPedidoVenda(long idPedidoVenda, string idUsuarioOperacao, long idEmpresa)
        {
            // validações
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

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao &&
                pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("O pedido de venda não está liberado para separação.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao &&
                pedidoVenda.IdUsuarioSeparacao != idUsuarioOperacao)
            {
                throw new BusinessException("O pedido já está sendo separado por outro usuário.");
            }

            // update do pedido na base oracle
            using (var transaction = _unitOfWork.CreateTransactionScope())
            {
                pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;
                pedidoVenda.IdUsuarioSeparacao = idUsuarioOperacao;
                pedidoVenda.DataHoraInicioSeparacao = DateTime.Now;

                _unitOfWork.PedidoVendaRepository.Update(pedidoVenda);

                _unitOfWork.SaveChanges();

                // update no Sankhya
                await AtualizarStatusPedidoVenda(pedidoVenda.Pedido, PedidoVendaStatusEnum.ProcessandoSeparacao);

                transaction.Complete();
            }
        }
    }
}