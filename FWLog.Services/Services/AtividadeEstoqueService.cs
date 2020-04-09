﻿using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Model.Armazenagem;
using FWLog.Services.Model.AtividadeEstoque;
using FWLog.Services.Model.Coletor;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class AtividadeEstoqueService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILog _log;
        private readonly ColetorHistoricoService _coletorHistoricoService;
        private readonly ArmazenagemService _armazenagemService;

        public AtividadeEstoqueService(UnitOfWork unitOfWork, ColetorHistoricoService coletorHistoricoService, ILog log, ArmazenagemService armazenagemService)
        {
            _unitOfWork = unitOfWork;
            _coletorHistoricoService = coletorHistoricoService;
            _log = log;
            _armazenagemService = armazenagemService;
        }

        public void ValidarCadastroAtividade(long idEmpresa)
        {
            if (idEmpresa <= 0)
            {
                throw new BusinessException("A empresa deve ser informada.");
            }
        }

        public void ValidarAtualizacaoAtividade(AtividadeEstoqueRequisicao atividadeEstoqueRequisicao)
        {
            if (atividadeEstoqueRequisicao.IdAtividadeEstoque <= 0)
            {
                throw new BusinessException("A atividade informada é inválida.");
            }

            var atividade = _unitOfWork.AtividadeEstoqueRepository.GetById(atividadeEstoqueRequisicao.IdAtividadeEstoque);

            if (atividade == null)
            {
                throw new BusinessException("A atividade informada não foi encontrada.");
            }

            var usuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(atividadeEstoqueRequisicao.IdUsuarioExecucao);

            if (usuario == null)
            {
                throw new BusinessException("O usuário informando nao foi encontrado.");
            }
        }

        public void CadastrarAtividadeAbastecerPicking(long idEmpresa)
        {
            try
            {
                var listaEnderecoSeparacao = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEmpresa(idEmpresa).Where(
                x => x.EnderecoArmazenagem.IsPontoSeparacao && (x.ProdutoEstoque.IdProdutoEstoqueStatus != ProdutoEstoqueStatusEnum.ForaLinha || x.ProdutoEstoque.IdProdutoEstoqueStatus != ProdutoEstoqueStatusEnum.LiquidacaoEstoque));

                if (listaEnderecoSeparacao != null)
                {
                    var listaAtividadeEstoque = listaEnderecoSeparacao.GroupBy(x => new { x.IdEmpresa, x.IdEnderecoArmazenagem, x.IdProduto, x.EnderecoArmazenagem.EstoqueMinimo, x.Quantidade }).Select(s => new AtividadeEstoqueLista()
                    {
                        IdEmpresa = s.Key.IdEmpresa,
                        IdEnderecoArmazenagem = s.Key.IdEnderecoArmazenagem,
                        IdProduto = s.Key.IdProduto,
                        Quantidade = s.Key.Quantidade,
                        EstoqueMinimo = s.Key.EstoqueMinimo
                    });

                    listaAtividadeEstoque = listaAtividadeEstoque.Where(x => x.Quantidade <= x.EstoqueMinimo);

                    foreach (var item in listaAtividadeEstoque)
                    {
                        var atividadeEstoque = _unitOfWork.AtividadeEstoqueRepository.Pesquisar(item.IdEmpresa, AtividadeEstoqueTipoEnum.AbastecerPicking, item.IdEnderecoArmazenagem,
                            item.IdProduto, false);

                        if (atividadeEstoque == null)
                        {
                            _unitOfWork.AtividadeEstoqueRepository.Add(new AtividadeEstoque()
                            {
                                IdEmpresa = item.IdEmpresa,
                                IdAtividadeEstoqueTipo = AtividadeEstoqueTipoEnum.AbastecerPicking,
                                IdEnderecoArmazenagem = item.IdEnderecoArmazenagem,
                                IdProduto = item.IdProduto,
                                DataSolicitacao = new DateTime(),
                                Finalizado = false
                            });

                            _unitOfWork.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Erro na criação das atividaes de abastecimento do picking.", ex);
            }
        }

        public void CadastrarAtividadeConferenciaEndereco(long idEmpresa)
        {
            try
            {
                DateTime dataAtual = new DateTime();
                int diaSemana = (int)dataAtual.DayOfWeek;

                if (diaSemana == 2) //Segunda-feira
                {
                    var listaEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEmpresa(idEmpresa).Where(x => x.Quantidade > 0);

                    foreach (var item in listaEndereco)
                    {
                        var atividadeEstoque = _unitOfWork.AtividadeEstoqueRepository.Pesquisar(item.IdEmpresa, AtividadeEstoqueTipoEnum.ConferenciaEndereco, item.IdEnderecoArmazenagem,
                            item.IdProduto, false);

                        if (atividadeEstoque == null)
                        {
                            _unitOfWork.AtividadeEstoqueRepository.Add(new Data.Models.AtividadeEstoque()
                            {
                                IdEmpresa = item.IdEmpresa,
                                IdAtividadeEstoqueTipo = AtividadeEstoqueTipoEnum.ConferenciaEndereco,
                                IdEnderecoArmazenagem = item.IdEnderecoArmazenagem,
                                IdProduto = item.IdProduto,
                                DataSolicitacao = new DateTime(),
                                Finalizado = false
                            });

                            _unitOfWork.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Erro na criação das atividades de conferência de endereço.", ex);
            }
        }

        public void CadastrarAtividadeConferecia399_400(long idEmpresa)
        {
            try
            {
                DateTime dataAtual = new DateTime();
                int diaSemana = (int)dataAtual.DayOfWeek;

                if (diaSemana != 1 && diaSemana != 7) //Diferente de sabado e domingo
                {
                    var listaEndereco = _unitOfWork.ProdutoEstoqueRepository.ObterProdutoEstoquePorEmpresa(idEmpresa).Where(x => x.Saldo == 0 && x.IdEnderecoArmazenagem.HasValue &&
                    (x.IdProdutoEstoqueStatus == ProdutoEstoqueStatusEnum.LiquidacaoEstoque || x.IdProdutoEstoqueStatus == ProdutoEstoqueStatusEnum.ForaLinha));

                    foreach (var item in listaEndereco)
                    {
                        var atividadeEstoque = _unitOfWork.AtividadeEstoqueRepository.Pesquisar(item.IdEmpresa, AtividadeEstoqueTipoEnum.ConferenciaProdutoForaLinha, item.IdEnderecoArmazenagem.Value,
                            item.IdProduto, false);

                        if (atividadeEstoque == null)
                        {
                            _unitOfWork.AtividadeEstoqueRepository.Add(new AtividadeEstoque()
                            {
                                IdEmpresa = item.IdEmpresa,
                                IdAtividadeEstoqueTipo = AtividadeEstoqueTipoEnum.ConferenciaProdutoForaLinha,
                                IdEnderecoArmazenagem = item.IdEnderecoArmazenagem.Value,
                                IdProduto = item.IdProduto,
                                DataSolicitacao = new DateTime(),
                                Finalizado = false
                            });

                            _unitOfWork.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Erro na criação das atividades de conferência 399/400.", ex);
            }
        }

        public void AtualizarAtividade(AtividadeEstoqueRequisicao atividadeEstoqueRequisicao)
        {
            try
            {
                var atividade = _unitOfWork.AtividadeEstoqueRepository.GetById(atividadeEstoqueRequisicao.IdAtividadeEstoque);

                if (atividade != null)
                {
                    atividade.QuantidadeInicial = atividadeEstoqueRequisicao.QuantidadeInicial;
                    atividade.QuantidadeFinal = atividadeEstoqueRequisicao.QuantidadeFinal;
                    atividade.IdUsuarioExecucao = atividadeEstoqueRequisicao.IdUsuarioExecucao;
                    atividade.DataExecucao = DateTime.Now;
                    atividade.Finalizado = true;

                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Erro na atualização da atividade {atividadeEstoqueRequisicao.IdAtividadeEstoque}", ex);
            }
        }

        public List<AtividadeEstoqueListaLinhaTabela> PesquisarAtividade(long idEmpresa, string idUsuario, int idAtividadeEstoqueTipo)
        {
            return _unitOfWork.AtividadeEstoqueRepository.PesquisarAtividade(idEmpresa, idUsuario, idAtividadeEstoqueTipo);
        }

        public void ValidarProdutoConferenciaProdutoForaLinha(int corredor, long idProduto, long idEmpresa)
        {
            if (corredor <= 0)
            {
                throw new BusinessException("O corredor deve ser informado.");
            }

            if (idProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto informado não foi encontrado.");
            }

            var listaDeAtividadesEstoque = _unitOfWork.AtividadeEstoqueRepository.PesquisarProdutosPendentes(idEmpresa, AtividadeEstoqueTipoEnum.ConferenciaProdutoForaLinha, idProduto);

            if (listaDeAtividadesEstoque.NullOrEmpty())
            {
                throw new BusinessException("Não foi encontrada atividade de estoque pendente para produto.");
            }

            if (!listaDeAtividadesEstoque.Any(ae => ae.EnderecoArmazenagem.Corredor == corredor))
            {
                throw new BusinessException("Produto não encontrado em corredor para conferência.");
            }
        }

        public void ValidarEnderecoConferenciaProdutoForaLinha(int corredor, long idEnderecoArmazenagem, long idProduto, long idEmpresa)
        {
            ValidarProdutoConferenciaProdutoForaLinha(corredor, idProduto, idEmpresa);

            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("O endereço informado não foi encontrado.");
            }

            var atividadeEstoque = _unitOfWork.AtividadeEstoqueRepository.Pesquisar(idEmpresa,
                                                                                            AtividadeEstoqueTipoEnum.ConferenciaProdutoForaLinha,
                                                                                            idEnderecoArmazenagem,
                                                                                            idProduto,
                                                                                            false);

            if (atividadeEstoque == null)
            {
                throw new BusinessException("Não foi encontrado produto com atividade de estoque pendente no endereço.");
            }
        }

        public void ValidarQuantidadeConferenciaProdutoForaLinha(int corredor, long idEnderecoArmazenagem, long idProduto, int quantidade, long idEmpresa)
        {
            ValidarEnderecoConferenciaProdutoForaLinha(corredor, idEnderecoArmazenagem, idProduto, idEmpresa);

            if (quantidade <= 0)
            {
                throw new BusinessException("A quantidade deve ser informada.");
            }

            var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(idProduto, idEmpresa);

            if (produtoEstoque == null)
            {
                throw new BusinessException("A quantidade deve ser informada.");
            }

            int quantidadeMaxima;

            if (produtoEstoque.Saldo == 0)
            {
                quantidadeMaxima = 3;
            }
            else
            {
                quantidadeMaxima = produtoEstoque.Saldo + ((produtoEstoque.Saldo * 30) / 100);
            }

            if (quantidade > quantidadeMaxima)
            {
                throw new BusinessException("Quantidade digitada maior que o permitido! Procure coordenação.");
            }
        }

        public async Task FinalizarConferenciaProdutoForaLinhaRequisicao(int corredor, long idEnderecoArmazenagem, long idProduto, int? quantidade, long idEmpresa, string idUsuarioExecucao)
        {
            if (quantidade.HasValue)
            {
                ValidarQuantidadeConferenciaProdutoForaLinha(corredor, idEnderecoArmazenagem, idProduto, quantidade.Value, idEmpresa);
            }
            else
            {
                ValidarEnderecoConferenciaProdutoForaLinha(corredor, idEnderecoArmazenagem, idProduto, idEmpresa);
            }

            var adicionaLogAuditoria = true;

            if (quantidade == null)
            {
                var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(idProduto, idEmpresa);

                if (produtoEstoque.Saldo == 0)
                {
                    adicionaLogAuditoria = false;
                }
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var atividadeEstoque = _unitOfWork.AtividadeEstoqueRepository.Pesquisar(idEmpresa,
                                                                                            AtividadeEstoqueTipoEnum.ConferenciaProdutoForaLinha,
                                                                                            idEnderecoArmazenagem,
                                                                                            idProduto,
                                                                                            false);

                var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

                var quantidadeFinal = quantidade ?? 0;

                atividadeEstoque.QuantidadeInicial = loteProdutoEndereco.Quantidade;
                atividadeEstoque.QuantidadeFinal = quantidadeFinal;
                atividadeEstoque.IdUsuarioExecucao = idUsuarioExecucao;
                atividadeEstoque.DataExecucao = DateTime.Now;
                atividadeEstoque.Finalizado = true;

                await _unitOfWork.SaveChangesAsync();

                //Aqui deve ser feito a solicitação de contagem

                if (quantidade.HasValue)
                {
                    if (loteProdutoEndereco.IdLote == null)
                    {
                        loteProdutoEndereco.Quantidade = quantidadeFinal;

                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        await _armazenagemService.AjustarVolumeLote(new AjustarVolumeLoteRequisicao()
                        {
                            IdEmpresa = idEmpresa,
                            IdEnderecoArmazenagem = idEnderecoArmazenagem,
                            IdLote = loteProdutoEndereco.IdLote.Value,
                            IdProduto = idProduto,
                            IdUsuarioAjuste = idUsuarioExecucao,
                            Quantidade = quantidadeFinal
                        });
                    }
                }

                if (adicionaLogAuditoria)
                {
                    _unitOfWork.ColetorHistoricoRepository.Add(new ColetorHistorico()
                    {
                        IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                        IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ConferirProdutoForaLinha,
                        DataHora = DateTime.Now,
                        Descricao = $"Conferiu 399/400 o produto {atividadeEstoque.Produto.Referencia} no endereço {atividadeEstoque.EnderecoArmazenagem.Codigo}," +
                        $" quantidade foi de {atividadeEstoque.QuantidadeInicial} para {atividadeEstoque.QuantidadeFinal}",
                        IdEmpresa = idEmpresa,
                        IdUsuario = idUsuarioExecucao
                    });

                    await _unitOfWork.SaveChangesAsync();
                }

                transacao.Complete();
            }
        }

        public async Task FinalizarConferenciaEnderecoRequisicao(FinalizarConferenciaEnderecoRequisicao requisicao, long idEmpresa, string idUsuarioExecucao)
        {
            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(requisicao.IdEnderecoArmazenagem);

                var qtdAnterior = loteProdutoEndereco.Quantidade;

                await _armazenagemService.AjustarVolumeLote(new AjustarVolumeLoteRequisicao
                {
                    IdEmpresa = idEmpresa,
                    IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                    IdLote = requisicao.IdLote,
                    IdProduto = requisicao.IdProduto,
                    IdUsuarioAjuste = idUsuarioExecucao,
                    Quantidade = requisicao.QuantidadeFinal
                });

                await _unitOfWork.SaveChangesAsync();

                var atividadeEstoque = _unitOfWork.AtividadeEstoqueRepository.GetById(requisicao.IdAtividadeEstoque);

                atividadeEstoque.QuantidadeInicial = qtdAnterior;
                atividadeEstoque.QuantidadeFinal = requisicao.QuantidadeFinal;
                atividadeEstoque.IdUsuarioExecucao = idUsuarioExecucao;
                atividadeEstoque.DataExecucao = DateTime.Now;
                atividadeEstoque.Finalizado = true;

                await _unitOfWork.SaveChangesAsync();

                _coletorHistoricoService.GravarHistoricoColetor(new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ConferirEndereco,
                    Descricao = $"Conferiu o produto {atividadeEstoque.Produto.Referencia} no endereço {atividadeEstoque.EnderecoArmazenagem.Codigo}," +
                       $" quantidade foi de {atividadeEstoque.QuantidadeInicial} para {atividadeEstoque.QuantidadeFinal}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioExecucao
                });

                await _unitOfWork.SaveChangesAsync();

                transacao.Complete();
            }
        }
    }
}