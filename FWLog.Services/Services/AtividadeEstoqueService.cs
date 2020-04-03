using FWLog.Data;
<<<<<<< HEAD
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Model.AtividadeEstoque;
using System;
using System.Linq;
using FWLog.Data.Models.DataTablesCtx;
using System.Collections.Generic;

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

        public void CadastrarAtividadeAbastecerPicking(long idEmpresa)
        {
            try
            {
                var listaEnderecoSeparacao = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEmpresa(idEmpresa).Where(
                x => x.EnderecoArmazenagem.IsPontoSeparacao && (x.ProdutoEstoque.IdProdutoEstoqueStatus != ProdutoEstoqueStatusEnum.ForaLinha || x.ProdutoEstoque.IdProdutoEstoqueStatus != ProdutoEstoqueStatusEnum.LiquidacaoEstoque));

                if (listaEnderecoSeparacao != null)
                {
                    IQueryable<AtividadeEstoqueLista> query = listaEnderecoSeparacao.GroupBy(x => new { x.IdEmpresa, x.IdEnderecoArmazenagem, x.IdProduto, x.EnderecoArmazenagem.EstoqueMinimo, x.Quantidade }).Select(s => new AtividadeEstoqueLista()
                    {
                        IdEmpresa = s.Key.IdEmpresa,
                        IdEnderecoArmazenagem = s.Key.IdEnderecoArmazenagem,
                        IdProduto = s.Key.IdProduto,
                        Quantidade = s.Key.Quantidade,
                        EstoqueMinimo = s.Key.EstoqueMinimo
                    });

                    query = query.Where(x => x.Quantidade <= x.EstoqueMinimo);

                    var listaAtividadeEstoque = query.ToList();

                    foreach (var item in listaAtividadeEstoque)
                    {
                        var atividadeEstoque = _unitOfWork.AtividadeEstoqueRepository.Pesquisar(item.IdEmpresa, AtividadeEstoqueTipoEnum.AbastecerPicking, item.IdEnderecoArmazenagem,
                            item.IdProduto, false);

                        if (atividadeEstoque == null)
                        {
                            _unitOfWork.AtividadeEstoqueRepository.Add(new Data.Models.AtividadeEstoque()
                            {
                                IdEmpresa = item.IdEmpresa,
                                IdAtividadeEstoqueTipo = AtividadeEstoqueTipoEnum.AbastecerPicking,
                                IdEnderecoArmazenagem = item.IdEnderecoArmazenagem,
                                IdProduto = item.IdProduto,
                                DataSolicitacao = new DateTime(),
                                Finalizado = false
                            });
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

        public void CadastrarrAtividadeConferecia399_400(long idEmpresa)
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
                            _unitOfWork.AtividadeEstoqueRepository.Add(new Data.Models.AtividadeEstoque()
                            {
                                IdEmpresa = item.IdEmpresa,
                                IdAtividadeEstoqueTipo = AtividadeEstoqueTipoEnum.ConferenciaProdutoForaLinha,
                                IdEnderecoArmazenagem = item.IdEnderecoArmazenagem.Value,
                                IdProduto = item.IdProduto,
                                DataSolicitacao = new DateTime(),
                                Finalizado = false
                            });
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

        public List<AtividadeEstoqueListaLinhaTabela> PesquisarAtividade(long idEmpresa)
        {
            return _unitOfWork.AtividadeEstoqueRepository.PesquisarAtividade(idEmpresa);
        }
    }
}
