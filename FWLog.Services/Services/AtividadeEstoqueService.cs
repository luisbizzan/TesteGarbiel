using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Model.AtividadeEstoque;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Services.Services
{
    public class AtividadeEstoqueService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;

        public AtividadeEstoqueService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                var applicationLogService = new ApplicationLogService(_unitOfWork);
                applicationLogService.Error(ApplicationEnum.Api, ex, "Erro na criação das atividaes de abastecimento do picking.");
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
                var applicationLogService = new ApplicationLogService(_unitOfWork);
                applicationLogService.Error(ApplicationEnum.Api, ex, "Erro na criação das atividades de conferência de endereço.");
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
                var applicationLogService = new ApplicationLogService(_unitOfWork);
                applicationLogService.Error(ApplicationEnum.Api, ex, "Erro na criação das atividades de conferência 399/400.");
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
                var applicationLogService = new ApplicationLogService(_unitOfWork);
                applicationLogService.Error(ApplicationEnum.Api, ex, "Erro na atualização da atividade " + atividadeEstoqueRequisicao.IdAtividadeEstoque);
            }
        }

        public List<AtividadeEstoqueListaLinhaTabela> PesquisarAtividade(long idEmpresa)
        {
            return _unitOfWork.AtividadeEstoqueRepository.PesquisarAtividade(idEmpresa);
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
                throw new BusinessException("Não foi encontrado produto com atividade de estoque pendente.");
            }

            if (!listaDeAtividadesEstoque.Any(ae => ae.EnderecoArmazenagem.Corredor == corredor))
            {
                throw new BusinessException("Produto não encontrado em corredor para conferência.");
            }
        }
    }
}