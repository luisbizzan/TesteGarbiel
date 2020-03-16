using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Armazenagem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ArmazenagemService
    {
        private readonly UnitOfWork _unitOfWork;

        public ArmazenagemService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            decimal pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * requisicao.Quantidade;

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
        }

        public void ValidarLoteInstalacao(ValidarLoteInstalacaoRequisicao requisicao)
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
                throw new BusinessException("A emrpesa não foi encontrada.");
            }

            Lote lote = _unitOfWork.LoteRepository.PesquisarPorLoteEmpresa(requisicao.IdLote, requisicao.IdEmpresa);

            if (lote == null)
            {
                throw new BusinessException("O lote não foi encontrado.");
            }
        }

        public void ValidarLoteProdutoInstalacao(ValidarLoteProdutoInstalacaoRequisicao requisicao)
        {
            var validarLoteRequisicao = new ValidarLoteInstalacaoRequisicao
            {
                IdLote = requisicao.IdLote,
                IdEmpresa = requisicao.IdEmpresa
            };

            ValidarLoteInstalacao(validarLoteRequisicao);

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

            var validarLoteRequisicao = new ValidarLoteInstalacaoRequisicao
            {
                IdLote = requisicao.IdLote,
                IdEmpresa = requisicao.IdEmpresa
            };

            ValidarLoteInstalacao(validarLoteRequisicao);

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

            var validarLoteRequisicao = new ValidarLoteInstalacaoRequisicao
            {
                IdLote = requisicao.IdLote,
                IdEmpresa = requisicao.IdEmpresa
            };

            ValidarLoteInstalacao(validarLoteRequisicao);

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

        public async Task RetirarVolumeEndereco(long idEnderecoArmazenagem, long idLote, long idProduto, long idEmpresa, string idUsuarioInstalacao)
        {
            ValidarProdutoRetirar(idEnderecoArmazenagem, idLote, idProduto, idEmpresa);

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var volume = ConsultaDetalhesVolumeInformado(idEnderecoArmazenagem, idLote, idProduto, idEmpresa);

                _unitOfWork.LoteProdutoEnderecoRepository.Delete(volume);
                await _unitOfWork.SaveChangesAsync();

                var loteMovimentacao = new LoteMovimentacao
                {
                    IdEmpresa = idEmpresa,
                    IdLote = idLote,
                    IdProduto = idProduto,
                    IdEnderecoArmazenagem = idEnderecoArmazenagem,
                    IdUsuarioMovimentacao = idUsuarioInstalacao,
                    Quantidade = volume.Quantidade,
                    IdLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Saida,
                    DataHora = DateTime.Now
                };

                _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                await _unitOfWork.SaveChangesAsync();
                transacao.Complete();
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
    }
}