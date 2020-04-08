using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Model.Armazenagem;
using FWLog.Services.Model.Coletor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ArmazenagemService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ColetorHistoricoService _coletorHistoricoService;

        public ArmazenagemService(UnitOfWork unitOfWork, ColetorHistoricoService coletorHistoricoService)
        {
            _unitOfWork = unitOfWork;
            _coletorHistoricoService = coletorHistoricoService;
        }

        public async Task InstalarVolumeLote(InstalarVolumeLoteRequisicao requisicao)
        {
            var validarEnderecoInstalacaoRequisicao = new ValidarEnderecoInstalacaoRequisicao
            {
                IdEmpresa = requisicao.IdEmpresa,
                IdLote = requisicao.IdLote,
                IdProduto = requisicao.IdProduto,
                Quantidade = requisicao.Quantidade,
                IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem
            };

            ValidarEnderecoInstalacao(validarEnderecoInstalacaoRequisicao);

            Produto produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);
            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(requisicao.IdEnderecoArmazenagem);
            decimal pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * requisicao.Quantidade;

            try
            {
                using (var transacao = _unitOfWork.CreateTransactionScope())
                {
                    var loteProdutoEndereco = new LoteProdutoEndereco
                    {
                        IdEmpresa = requisicao.IdEmpresa,
                        DataHoraInstalacao = DateTime.Now,
                        IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                        IdLote = requisicao.IdLote,
                        IdProduto = requisicao.IdProduto,
                        IdUsuarioInstalacao = requisicao.IdUsuarioInstalacao,
                        Quantidade = requisicao.Quantidade,
                        PesoTotal = pesoInstalacao
                    };

                    _unitOfWork.LoteProdutoEnderecoRepository.Add(loteProdutoEndereco);
                    await _unitOfWork.SaveChangesAsync();

                    var loteMovimentacao = new LoteMovimentacao
                    {
                        IdEmpresa = requisicao.IdEmpresa,
                        IdLote = requisicao.IdLote,
                        IdProduto = requisicao.IdProduto,
                        IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                        IdUsuarioMovimentacao = requisicao.IdUsuarioInstalacao,
                        Quantidade = requisicao.Quantidade,
                        IdLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Entrada,
                        DataHora = DateTime.Now
                    };

                    _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                    await _unitOfWork.SaveChangesAsync();
                    transacao.Complete();
                }

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.InstalarProduto,
                    Descricao = $"Instalou o produto {produto.Referencia} do lote {requisicao.IdLote} no endereço {enderecoArmazenagem.Codigo}",
                    IdEmpresa = requisicao.IdEmpresa,
                    IdUsuario = requisicao.IdUsuarioInstalacao
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
            }
            catch
            {
                throw new BusinessException($"Erro ao instalar o produto {produto.Referencia} do lote {requisicao.IdLote} no endereço {enderecoArmazenagem.Codigo}");
            }
        }

        public void ValidarLote(ValidarLoteRequisicao requisicao)
        {
            if (requisicao.IdLote <= 0)
            {
                throw new BusinessException("O lote deve ser informado.");
            }

            if (requisicao.IdEmpresa <= 0)
            {
                throw new BusinessException("A empresa deve ser informada.");
            }

            Empresa empresa = _unitOfWork.EmpresaRepository.GetById(requisicao.IdEmpresa);

            if (empresa == null)
            {
                throw new BusinessException("A empresa não foi encontrada.");
            }

            Lote lote = _unitOfWork.LoteRepository.PesquisarPorLoteEmpresa(requisicao.IdLote, requisicao.IdEmpresa);

            if (lote == null)
            {
                throw new BusinessException("O lote não foi encontrado.");
            }
        }

        public void ValidarLoteProdutoInstalacao(ValidarLoteProdutoInstalacaoRequisicao requisicao)
        {
            var validarLoteRequisicao = new ValidarLoteRequisicao
            {
                IdLote = requisicao.IdLote,
                IdEmpresa = requisicao.IdEmpresa
            };

            ValidarLote(validarLoteRequisicao);

            if (requisicao.IdProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            Produto produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            LoteProduto loteProduto = _unitOfWork.LoteProdutoRepository.PesquisarProdutoNoLote(requisicao.IdEmpresa, requisicao.IdLote, requisicao.IdProduto);

            if (loteProduto == null)
            {
                throw new BusinessException("O produto não pertence ao lote.");
            }

            if (loteProduto.Saldo == 0)
            {
                throw new BusinessException("Saldo do produto no lote insuficiente.");
            }
        }

        public void ValidarQuantidadeInstalacao(ValidarQuantidadeInstalacaoRequisicao requisicao)
        {
            if (requisicao.Quantidade <= 0)
            {
                throw new BusinessException("A quantidade deve ser informada.");
            }

            var validarLoteRequisicao = new ValidarLoteRequisicao
            {
                IdLote = requisicao.IdLote,
                IdEmpresa = requisicao.IdEmpresa
            };

            ValidarLote(validarLoteRequisicao);

            var validarLoteProdutoRequisicao = new ValidarLoteProdutoInstalacaoRequisicao
            {
                IdEmpresa = requisicao.IdEmpresa,
                IdLote = requisicao.IdLote,
                IdProduto = requisicao.IdProduto
            };

            ValidarLoteProdutoInstalacao(validarLoteProdutoRequisicao);

            var listaEnderecosLoteProduto = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorLoteProduto(requisicao.IdLote, requisicao.IdProduto);

            LoteProduto loteProduto = _unitOfWork.LoteProdutoRepository.PesquisarProdutoNoLote(requisicao.IdEmpresa, requisicao.IdLote, requisicao.IdProduto);

            int saldoLote = loteProduto.Saldo;
            int totalInstalado = listaEnderecosLoteProduto.Sum(s => s.Quantidade);

            if ((totalInstalado + requisicao.Quantidade) > saldoLote)
            {
                throw new BusinessException("Quantidade maior que o saldo do produto no lote.");
            }
        }

        public void ValidarEnderecoInstalacao(ValidarEnderecoInstalacaoRequisicao requisicao)
        {
            if (requisicao.IdEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            var validarLoteRequisicao = new ValidarLoteRequisicao
            {
                IdLote = requisicao.IdLote,
                IdEmpresa = requisicao.IdEmpresa
            };

            ValidarLote(validarLoteRequisicao);

            var validarLoteProdutoRequisicao = new ValidarLoteProdutoInstalacaoRequisicao
            {
                IdEmpresa = requisicao.IdEmpresa,
                IdLote = requisicao.IdLote,
                IdProduto = requisicao.IdProduto
            };

            ValidarLoteProdutoInstalacao(validarLoteProdutoRequisicao);

            var validarQuantidadeRequisicao = new ValidarQuantidadeInstalacaoRequisicao
            {
                IdEmpresa = requisicao.IdEmpresa,
                IdLote = requisicao.IdLote,
                IdProduto = requisicao.IdProduto,
                Quantidade = requisicao.Quantidade
            };

            ValidarQuantidadeInstalacao(validarQuantidadeRequisicao);

            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(requisicao.IdEnderecoArmazenagem);

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("O endereço não foi encontrado.");
            }

            if (enderecoArmazenagem.Ativo == false)
            {
                throw new BusinessException("O endereço não está ativo.");
            }

            LoteProdutoEndereco loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(requisicao.IdEnderecoArmazenagem);

            if (loteProdutoEndereco != null)
            {
                throw new BusinessException("O endereço já está ocupado.");
            }

            Produto produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);
            decimal pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * requisicao.Quantidade;

            ValidarPeso(enderecoArmazenagem, pesoInstalacao);
        }

        private void ValidarPeso(EnderecoArmazenagem enderecoArmazenagem, decimal pesoInstalacao)
        {
            //limite de peso do endereço
            if (enderecoArmazenagem.LimitePeso.HasValue)
            {
                if (pesoInstalacao > enderecoArmazenagem.LimitePeso)
                {
                    throw new BusinessException("Quantidade ultrapassa limite de peso do endereço.");
                }
            }

            //limite de peso vertical do ponto
            if (enderecoArmazenagem.PontoArmazenagem.LimitePesoVertical.HasValue)
            {
                List<EnderecoArmazenagem> enderecosPontoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarPorPontoArmazenagem(enderecoArmazenagem.IdPontoArmazenagem);

                List<long> enderecosVertical = enderecosPontoArmazenagem.
                    Where(w => w.Corredor == enderecoArmazenagem.Corredor && w.Vertical == enderecoArmazenagem.Vertical && w.Ativo).
                    Select(w => w.IdEnderecoArmazenagem).ToList();

                List<LoteProdutoEndereco> lotesProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecos(enderecosVertical);

                decimal pesoVerticalInstalado = lotesProdutoEndereco.Sum(s => s.PesoTotal);
                decimal limitePesoVerticalPonto = enderecoArmazenagem.PontoArmazenagem.LimitePesoVertical.Value;

                if (pesoVerticalInstalado + pesoInstalacao > limitePesoVerticalPonto)
                {
                    throw new BusinessException("Quantidade ultrapassa limite de peso vertical.");
                }
            }
        }

        public void ValidarEndereco(long idEnderecoArmazenagem)
        {
            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("O endereço não foi encontrado.");
            }

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

            if (loteProdutoEndereco == null)
            {
                throw new BusinessException("Nenhum volume instalado no endereço.");
            }
        }

        public void ValidateLoteRetirar(long idEnderecoArmazenagem, long idLote)
        {
            ValidarEndereco(idEnderecoArmazenagem);

            if (idLote <= 0)
            {
                throw new BusinessException("O lote deve ser informado.");
            }

            var lote = _unitOfWork.LoteRepository.GetById(idLote);

            if (lote == null)
            {
                throw new BusinessException("O lote não foi encontrado.");
            }

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

            if (loteProdutoEndereco.IdLote != idLote)
            {
                throw new BusinessException("O lote não está instalado no endereço informado.");
            }
        }

        public void ValidarProdutoRetirar(long idEnderecoArmazenagem, long idLote, long idProduto, long idEmpresa)
        {
            ValidarEndereco(idEnderecoArmazenagem);

            ValidateLoteRetirar(idEnderecoArmazenagem, idLote);

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoLoteProdutoEmpresa(idEnderecoArmazenagem, idLote, idProduto, idEmpresa);

            if (loteProdutoEndereco == null)
            {
                throw new BusinessException("Não foi encontrado lote associado ao produto.");
            }

            if (loteProdutoEndereco.IdLote != idLote)
            {
                throw new BusinessException("Produto não pertence ao lote informado.");
            }

            if (loteProdutoEndereco.IdEnderecoArmazenagem != idEnderecoArmazenagem)
            {
                throw new BusinessException("Produto não está instalado no endereço informado.");
            }
        }

        public LoteProdutoEndereco ConsultaDetalhesVolumeInformado(long idEnderecoArmazenagem, long idLote, long idProduto, long idEmpresa)
        {
            var produtoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoLoteProdutoEmpresa(idEnderecoArmazenagem, idLote, idProduto, idEmpresa);

            if (produtoEndereco == null)
            {
                throw new BusinessException("Não foi encontrado volume com os dados informados.");
            }

            return produtoEndereco;
        }

        public async Task RetirarVolumeEndereco(long idEnderecoArmazenagem, long idLote, long idProduto, long idEmpresa, string idUsuarioRetirada)
        {
            ValidarProdutoRetirar(idEnderecoArmazenagem, idLote, idProduto, idEmpresa);

            var volume = ConsultaDetalhesVolumeInformado(idEnderecoArmazenagem, idLote, idProduto, idEmpresa);

            try
            {
                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.RetirarProduto,
                    Descricao = $"Retirou o produto {volume.Produto.Referencia} do lote {volume.Lote.IdLote} do endereço {volume.EnderecoArmazenagem.Codigo}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioRetirada
                };

                using (var transacao = _unitOfWork.CreateTransactionScope())
                {
                    _unitOfWork.LoteProdutoEnderecoRepository.Delete(volume);
                    await _unitOfWork.SaveChangesAsync();

                    var loteMovimentacao = new LoteMovimentacao
                    {
                        IdEmpresa = idEmpresa,
                        IdLote = idLote,
                        IdProduto = idProduto,
                        IdEnderecoArmazenagem = idEnderecoArmazenagem,
                        IdUsuarioMovimentacao = idUsuarioRetirada,
                        Quantidade = volume.Quantidade,
                        IdLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Saida,
                        DataHora = DateTime.Now
                    };

                    _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                    await _unitOfWork.SaveChangesAsync();
                    transacao.Complete();
                }

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
            }
            catch
            {
                throw new BusinessException($"Erro ao retirar o produto {volume.Produto.Referencia} do lote {volume.Lote.IdLote} e do endereco {volume.EnderecoArmazenagem.Codigo}.");
            }
        }

        public void ValidarEnderecoAjuste(ValidarEnderecoAjusteRequisicao requisicao)
        {
            if (requisicao.IdEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(requisicao.IdEnderecoArmazenagem);

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("O endereço não foi encontrado.");
            }

            var loteProduto = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(requisicao.IdEnderecoArmazenagem);

            if (loteProduto == null)
            {
                throw new BusinessException("Nenhum volume instalado no endereço.");
            }
        }

        public void ValidateLoteAjuste(long idEnderecoArmazenagem, long idLote)
        {
            ValidarEndereco(idEnderecoArmazenagem);

            if (idLote <= 0)
            {
                throw new BusinessException("O lote deve ser informado.");
            }

            var lote = _unitOfWork.LoteRepository.GetById(idLote);

            if (lote == null)
            {
                throw new BusinessException("O lote não foi encontrado.");
            }

            var loteProduto = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

            if (loteProduto.IdLote != idLote)
            {
                throw new BusinessException("O lote não está instalado no endereço informado.");
            }
        }

        public void ValidarProdutoAjuste(long idEnderecoArmazenagem, long idLote, long idProduto, long idEmpresa)
        {
            ValidateLoteAjuste(idEnderecoArmazenagem, idLote);

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoLoteProdutoEmpresa(idEnderecoArmazenagem, idLote, idProduto, idEmpresa);

            if (loteProdutoEndereco == null)
            {
                throw new BusinessException("Não foi encontrado lote associado ao produto.");
            }

            if (loteProdutoEndereco.IdLote != idLote)
            {
                throw new BusinessException("Produto não pertence ao lote informado.");
            }

            if (loteProdutoEndereco.IdEnderecoArmazenagem != idEnderecoArmazenagem)
            {
                throw new BusinessException("Produto não está instalado no endereço informado.");
            }
        }

        public void ValidarQuantidadeAjuste(ValidarQuantidadeAjusteRequisicao requisicao)
        {
            if (requisicao.Quantidade <= 0)
            {
                throw new BusinessException("A quantidade deve ser informada.");
            }

            var validarLoteRequisicao = new ValidarLoteRequisicao
            {
                IdLote = requisicao.IdLote,
                IdEmpresa = requisicao.IdEmpresa
            };

            ValidarLote(validarLoteRequisicao);

            var validarLoteProdutoRequisicao = new ValidarLoteProdutoAjusteRequisicao
            {
                IdEmpresa = requisicao.IdEmpresa,
                IdLote = requisicao.IdLote,
                IdProduto = requisicao.IdProduto
            };

            ValidarLoteProdutoAjuste(validarLoteProdutoRequisicao);

            var listaEnderecosLoteProduto = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorLoteProduto(requisicao.IdLote, requisicao.IdProduto);

            LoteProduto loteProduto = _unitOfWork.LoteProdutoRepository.PesquisarProdutoNoLote(requisicao.IdEmpresa, requisicao.IdLote, requisicao.IdProduto);

            int saldoLote = loteProduto.Saldo;
            int totalInstalado = listaEnderecosLoteProduto.Sum(s => s.Quantidade);

            if ((totalInstalado + requisicao.Quantidade) > saldoLote)
            {
                throw new BusinessException("Quantidade maior que o saldo do produto no lote.");
            }

            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(requisicao.IdEnderecoArmazenagem);
            Produto produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);
            decimal pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * requisicao.Quantidade;

            ValidarPeso(enderecoArmazenagem, pesoInstalacao);
        }

        public void ValidarLoteProdutoAjuste(ValidarLoteProdutoAjusteRequisicao requisicao)
        {
            var validarLoteRequisicao = new ValidarLoteRequisicao
            {
                IdLote = requisicao.IdLote,
                IdEmpresa = requisicao.IdEmpresa
            };

            ValidarLote(validarLoteRequisicao);

            if (requisicao.IdProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            Produto produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            LoteProduto loteProduto = _unitOfWork.LoteProdutoRepository.PesquisarProdutoNoLote(requisicao.IdEmpresa, requisicao.IdLote, requisicao.IdProduto);

            if (loteProduto == null)
            {
                throw new BusinessException("O produto não pertence ao lote.");
            }

            if (loteProduto.Saldo == 0)
            {
                throw new BusinessException("Saldo do produto no lote insuficiente.");
            }
        }

        public async Task AjustarVolumeLote(AjustarVolumeLoteRequisicao requisicao)
        {
            var validarEnderecoInstalacaoRequisicao = new ValidarEnderecoAjusteRequisicao
            {
                IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem
            };

            ValidarEnderecoAjuste(validarEnderecoInstalacaoRequisicao);

            ValidarProdutoAjuste(requisicao.IdEnderecoArmazenagem, requisicao.IdLote, requisicao.IdProduto, requisicao.IdEmpresa);

            ValidarQuantidadeAjuste(new ValidarQuantidadeAjusteRequisicao
            {
                IdEmpresa = requisicao.IdEmpresa,
                IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                IdLote = requisicao.IdLote,
                IdProduto = requisicao.IdProduto,
                Quantidade = requisicao.Quantidade
            });

            Produto produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);
            LoteProdutoEndereco produtoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(requisicao.IdEnderecoArmazenagem);
            decimal pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * requisicao.Quantidade;
            int quantidadeAnterior;

            try
            {
                using (var transacao = _unitOfWork.CreateTransactionScope())
                {
                    quantidadeAnterior = produtoEndereco.Quantidade;

                    produtoEndereco.Quantidade = requisicao.Quantidade;
                    produtoEndereco.PesoTotal = pesoInstalacao;

                    _unitOfWork.LoteProdutoEnderecoRepository.Update(produtoEndereco);
                    await _unitOfWork.SaveChangesAsync();

                    var loteMovimentacao = new LoteMovimentacao
                    {
                        IdEmpresa = requisicao.IdEmpresa,
                        IdLote = requisicao.IdLote,
                        IdProduto = requisicao.IdProduto,
                        IdEnderecoArmazenagem = requisicao.IdEnderecoArmazenagem,
                        IdUsuarioMovimentacao = requisicao.IdUsuarioAjuste,
                        Quantidade = requisicao.Quantidade,
                        IdLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Ajuste,
                        DataHora = DateTime.Now
                    };

                    _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                    await _unitOfWork.SaveChangesAsync();
                    transacao.Complete();
                }

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.AjustarQuantidade,
                    Descricao = $"Ajustou a quantidade de {quantidadeAnterior} para {produtoEndereco.Quantidade} unidade(s) do produto {produtoEndereco.Produto.Referencia} do lote {produtoEndereco.Lote.IdLote} do endereço {produtoEndereco.EnderecoArmazenagem.Codigo}",
                    IdEmpresa = requisicao.IdEmpresa,
                    IdUsuario = requisicao.IdUsuarioAjuste
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
            }
            catch
            {
                throw new BusinessException($"Erro ao ajustar a quantidade do produto {produto.Referencia} do lote {produtoEndereco.Lote.IdLote} e do endereço {produtoEndereco.EnderecoArmazenagem.Codigo}");
            }
        }

        public void ValidarEnderecoAbastecer(long idEnderecoArmazenagem)
        {
            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("O endereço não foi encontrado.");
            }

            if (!enderecoArmazenagem.Ativo)
            {
                throw new BusinessException("O endereço não está ativo.");
            }

            if (!enderecoArmazenagem.IsPontoSeparacao)
            {
                throw new BusinessException("O endereço não é um ponto de separação.");
            }

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

            if (loteProdutoEndereco == null)
            {
                throw new BusinessException("Nenhum volume instalado no endereço.");
            }

            if (enderecoArmazenagem.EstoqueMaximo.HasValue && loteProdutoEndereco.Quantidade >= enderecoArmazenagem.EstoqueMaximo.Value)
            {
                throw new BusinessException("Quantidade no endereço já atingiu o máximo permitido.");
            }
        }

        public LoteProdutoEndereco ConsultaDetalhesEnderecoArmazenagem(long idEnderecoArmazenagem)
        {
            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

            if (loteProdutoEndereco == null)
            {
                throw new BusinessException("Nenhum volume instalado no endereço.");
            }

            return loteProdutoEndereco;
        }

        public void ValidarLoteAbastecer(long idEnderecoArmazenagem, long idLote, long idProduto)
        {
            ValidarEnderecoAbastecer(idEnderecoArmazenagem);

            if (idLote <= 0)
            {
                throw new BusinessException("O lote deve ser informado.");
            }

            var lote = _unitOfWork.LoteRepository.GetById(idLote);

            if (lote == null)
            {
                throw new BusinessException("O lote não foi encontrado.");
            }

            if (idProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            var loteProduto = _unitOfWork.LoteProdutoRepository.ConsultarPorLoteProduto(idLote, idProduto);

            if (loteProduto == null)
            {
                throw new BusinessException("Não foi encontrado produto para esse lote.");
            }

            if (loteProduto.Saldo == 0)
            {
                throw new BusinessException("Não existem mais produtos disponíveis no lote.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            if (enderecoArmazenagem.IsFifo)
            {
                var loteMaisAntigo = _unitOfWork.LoteProdutoRepository.PesquisarProdutoMaisAntigoPorEmpresaESaldo(idProduto, loteProduto.IdEmpresa);

                if (loteMaisAntigo.IdLote != idLote)
                {
                    throw new BusinessException("Endereço controla FIFO. Lote informado não é o mais antigo.");
                }
            }
        }

        public void ValidarQuantidadeAbastecer(long idEnderecoArmazenagem, long idLote, long idProduto, int quantidade)
        {
            ValidarEnderecoAbastecer(idEnderecoArmazenagem);

            ValidarLoteAbastecer(idEnderecoArmazenagem, idLote, idProduto);

            if (quantidade <= 0)
            {
                throw new BusinessException("Quantidade deve ser maior que zero.");
            }

            var loteProduto = _unitOfWork.LoteProdutoRepository.ConsultarPorLoteProduto(idLote, idProduto);

            if (quantidade > loteProduto.Saldo)
            {
                throw new BusinessException("Quantidade deve ser menor que o saldo disponível.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            if (enderecoArmazenagem.EstoqueMaximo.HasValue)
            {
                var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

                var quantidadeAposAbastecer = quantidade + loteProdutoEndereco.Quantidade;

                if (quantidadeAposAbastecer > enderecoArmazenagem.EstoqueMaximo)
                {
                    throw new BusinessException("Quantidade informada e instalada devem ser menores que o estoque máximo do endereço.");
                }
            }
        }

        public async Task AbastecerPicking(long idEnderecoArmazenagem, long idLote, long idProduto, int quantidade, long idEmpresa, string idUsuarioOperacao)
        {
            ValidarEnderecoAbastecer(idEnderecoArmazenagem);

            ValidarLoteAbastecer(idEnderecoArmazenagem, idLote, idProduto);

            ValidarQuantidadeAbastecer(idEnderecoArmazenagem, idLote, idProduto, quantidade);

            var loteProduto = _unitOfWork.LoteProdutoRepository.ConsultarPorLoteProduto(idLote, idProduto);

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

            try
            {
                using (var transacao = _unitOfWork.CreateTransactionScope())
                {
                    loteProduto.Saldo -= quantidade;

                    _unitOfWork.LoteProdutoRepository.Update(loteProduto);
                    await _unitOfWork.SaveChangesAsync();

                    loteProdutoEndereco.Quantidade += quantidade;

                    _unitOfWork.LoteProdutoEnderecoRepository.Update(loteProdutoEndereco);
                    await _unitOfWork.SaveChangesAsync();

                    var loteMovimentacao = new LoteMovimentacao
                    {
                        IdEmpresa = idEmpresa,
                        IdLote = idLote,
                        IdProduto = idProduto,
                        IdEnderecoArmazenagem = idEnderecoArmazenagem,
                        IdUsuarioMovimentacao = idUsuarioOperacao,
                        Quantidade = quantidade,
                        IdLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Abastecimento,
                        DataHora = DateTime.Now
                    };

                    _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                    await _unitOfWork.SaveChangesAsync();

                    transacao.Complete();
                }

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.AjustarQuantidade,
                    Descricao = $"Abasteceu o produto {loteProdutoEndereco.Produto.Referencia} do lote {loteProduto.Lote.IdLote} no endereço de picking {loteProdutoEndereco.EnderecoArmazenagem.Codigo}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioOperacao
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
            }
            catch
            {
                throw new BusinessException($"Erro ao abastecer o picking com o produto {loteProduto.Produto.Referencia} do lote {loteProduto.Lote.IdLote}");
            }
        }

        public LoteInstaladoProdutoResposta PesquisaLotesInstaladosProduto(long idProduto)
        {
            if (idProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            var resposta = new LoteInstaladoProdutoResposta
            {
                IdProduto = produto.IdProduto,
                ReferenciaProduto = produto.Referencia,
                CodigoBarrasProduto = produto.CodigoBarras
            };

            resposta.ListaDatasUsuarios = new List<LoteInstaladoProdutoDataUsuario>();

            var listaLoteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorProdutoComLote(idProduto);

            var agrupamentoDataUsuario = listaLoteProdutoEndereco.GroupBy(g => new { g.DataHoraInstalacao, g.AspNetUsers }).ToList();

            foreach (var itemDataUsuario in agrupamentoDataUsuario)
            {
                var dataUsuario = new LoteInstaladoProdutoDataUsuario();

                dataUsuario.DataHoraInstalacao = itemDataUsuario.Key.DataHoraInstalacao;
                dataUsuario.CodigoUsuario = itemDataUsuario.Key.AspNetUsers.UserName;

                var agrupamentoLoteNivelPonto = itemDataUsuario.ToList().GroupBy(g => new
                {
                    IdLote = g.IdLote.Value,
                    g.EnderecoArmazenagem.IdNivelArmazenagem,
                    NivelArmazenagemDescricao = g.EnderecoArmazenagem.NivelArmazenagem.Descricao,
                    g.EnderecoArmazenagem.IdPontoArmazenagem,
                    PontoArmazenagemDescricao = g.EnderecoArmazenagem.PontoArmazenagem.Descricao
                }).ToList();

                dataUsuario.ListaLotes = new List<LoteInstaladoProdutoLoteNivelPonto>();

                foreach (var itemLoteNivelPonto in agrupamentoLoteNivelPonto)
                {
                    var loteNivelPonto = new LoteInstaladoProdutoLoteNivelPonto();

                    loteNivelPonto.IdLote = itemLoteNivelPonto.Key.IdLote;
                    loteNivelPonto.IdNivelArmazenagem = itemLoteNivelPonto.Key.IdNivelArmazenagem;
                    loteNivelPonto.DescricaoNivelArmazenagem = itemLoteNivelPonto.Key.NivelArmazenagemDescricao;
                    loteNivelPonto.IdPontoArmazenagem = itemLoteNivelPonto.Key.IdPontoArmazenagem;
                    loteNivelPonto.DescricaoPontoArmazenagem = itemLoteNivelPonto.Key.PontoArmazenagemDescricao;

                    loteNivelPonto.ListaEnderecos = new List<LoteInstaladoProdutoLoteNivelPontoEndereco>();

                    foreach (var itemEnderecoQuantidade in itemLoteNivelPonto.Select(ilnp => new
                    {
                        ilnp.EnderecoArmazenagem.IdEnderecoArmazenagem,
                        ilnp.EnderecoArmazenagem.Codigo,
                        ilnp.Quantidade
                    }).ToList())
                    {
                        loteNivelPonto.ListaEnderecos.Add(new LoteInstaladoProdutoLoteNivelPontoEndereco()
                        {
                            IdEnderecoArmazenagem = itemEnderecoQuantidade.IdEnderecoArmazenagem,
                            CodigoEnderecoArmazenagem = itemEnderecoQuantidade.Codigo,
                            Quantidade = itemEnderecoQuantidade.Quantidade
                        });
                    }

                    dataUsuario.ListaLotes.Add(loteNivelPonto);
                }

                resposta.ListaDatasUsuarios.Add(dataUsuario);
            }

            return resposta;
        }

        public LoteProdutoEndereco ValidarEnderecoConferir(long idEnderecoArmazenagem)
        {
            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            var enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("O endereço não foi encontrado.");
            }

            if (!enderecoArmazenagem.IsEntrada)
            {
                throw new BusinessException("O endereço não é um ponto de entrada.");
            }

            if (enderecoArmazenagem.PontoArmazenagem.IdTipoMovimentacao != TipoMovimentacaoEnum.Conferencia)
            {
                throw new BusinessException("Endereço não é um ponto de conferência.");
            }

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEndereco(idEnderecoArmazenagem);

            if (loteProdutoEndereco == null)
            {
                throw new BusinessException("Nenhum volume instalado no endereço.");
            }

            return loteProdutoEndereco;
        }

        public void ValidarProdutoConferir(long idEnderecoArmazenagem, long idProduto)
        {
            var volumeInstalado = ValidarEnderecoConferir(idEnderecoArmazenagem);

            if (idProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            if (volumeInstalado.IdProduto != idProduto)
            {
                throw new BusinessException("O produto não pertence ao lote instalado.");
            }
        }

        public async Task FinalizarConferencia(long idEnderecoArmazenagem, long idProduto, int quantidade, long idEmpresa, string idUsuarioOperacao)
        {
            var volume = ValidarEnderecoConferir(idEnderecoArmazenagem);

            ValidarProdutoConferir(idEnderecoArmazenagem, idProduto);

            if (quantidade != volume.Quantidade)
            {
                throw new BusinessException("Quantidade de produtos informada diverge da quantidade instalada.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var idLote = volume.IdLote.GetValueOrDefault();
                var referenciaProduto = volume.Produto.Referencia;
                var codigoEndereco = volume.EnderecoArmazenagem.Codigo;

                await RetirarVolumeEndereco(idEnderecoArmazenagem, idLote, volume.IdProduto, idEmpresa, idUsuarioOperacao);

                var loteMovimentacao = new LoteMovimentacao
                {
                    IdEmpresa = idEmpresa,
                    IdLote = idLote,
                    IdProduto = volume.IdProduto,
                    IdEnderecoArmazenagem = idEnderecoArmazenagem,
                    IdUsuarioMovimentacao = idUsuarioOperacao,
                    Quantidade = quantidade,
                    IdLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Conferencia,
                    DataHora = DateTime.Now
                };

                _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                await _unitOfWork.SaveChangesAsync();

                var coletorHistorico = new ColetorHistorico
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ConferirEndereco,
                    DataHora = DateTime.Now,
                    Descricao = $"Retirou o produto {referenciaProduto} do lote {idLote} do endereço {codigoEndereco} após conferência",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioOperacao
                };

                _unitOfWork.ColetorHistoricoTipoRepository.GravarHistorico(coletorHistorico);

                transacao.Complete();
            }
        }

        public List<EnderecoProdutoListaLinhaTabela> PesquisarNivelPontoCorredor(long idPontoArmazenagem, int corredor, long idEmpresa)
        {
            var enderecosArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarNivelPontoCorredor(corredor, idPontoArmazenagem, idEmpresa);

            if (enderecosArmazenagem == null)
            {
                throw new BusinessException("O corredor não foi encontrado.");
            }

            return enderecosArmazenagem;
        }

        public List<PontoArmazenagem> PesquisarPorCorredor(int corredor, long idEmpresa)
        {
            List<EnderecoArmazenagem> enderecosArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarPorCorredor(corredor, idEmpresa);

            if (enderecosArmazenagem == null)
            {
                throw new BusinessException("O corredor não foi encontrado.");
            }

            List<PontoArmazenagem> pontos = enderecosArmazenagem.Select(s => s.PontoArmazenagem).Distinct().ToList();

            return pontos;
        }
    }
}