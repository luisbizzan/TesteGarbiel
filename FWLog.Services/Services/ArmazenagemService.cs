using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Armazenagem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void ValidarLoteInstalacao(ValidarLoteInstalacaoRequisicao requisicao)
        {
            if(requisicao.IdLote <= 0)
            {
                throw new BusinessException("O lote deve ser informado.");
            }

            if(requisicao.IdEmpresa <= 0)
            {
                throw new BusinessException("A empresa deve ser informada.");
            }

            Empresa empresa = _unitOfWork.EmpresaRepository.GetById(requisicao.IdEmpresa);

            if(empresa == null)
            {
                throw new BusinessException("A emrpesa naõ foi encontrada.");
            }

            Lote lote = _unitOfWork.LoteRepository.PesquisarPorLoteEmpresa(requisicao.IdLote, requisicao.IdEmpresa);

            if(lote == null)
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

            if(requisicao.IdProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            Produto produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);

            if(produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            LoteProduto loteProduto = _unitOfWork.LoteProdutoRepository.PesquisarProdutoNoLote(requisicao.IdEmpresa, requisicao.IdLote, requisicao.IdProduto);

            if(loteProduto == null)
            {
                throw new BusinessException("O produto não pertence ao lote.");
            }

            if(loteProduto.Saldo == 0)
            {
                throw new BusinessException("Saldo do produto no lote insuficiente.");
            }
        }

        public void ValidarQuantidadeInstalacao(ValidarQuantidadeInstalacaoRequisicao requisicao)
        {
            if(requisicao.Quantidade <= 0)
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

            var listaEnderecosLoteProduto = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorLoteProduto(requisicao.IdEmpresa, requisicao.IdLote, requisicao.IdProduto);

            if(listaEnderecosLoteProduto.Count > 0)
            {
                LoteProduto loteProduto = _unitOfWork.LoteProdutoRepository.PesquisarProdutoNoLote(requisicao.IdEmpresa, requisicao.IdLote, requisicao.IdProduto);

                int saldoLote = loteProduto.Saldo;
                int totalInstalado = listaEnderecosLoteProduto.Sum(s => s.Quantidade);

                if(totalInstalado + requisicao.Quantidade > saldoLote)
                {
                    throw new BusinessException("Quantidade maior que o saldo do produto no lote.");
                }
            }
        }

        public void ValidarEnderecoInstalacao(ValidarEnderecoInstalacaoRequisicao requisicao)
        {
            if(requisicao.IdEnderecoArmazenagem <= 0)
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

            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.PesquisarPorEmpresaEndereco(requisicao.IdEmpresa, requisicao.IdEnderecoArmazenagem);

            if (enderecoArmazenagem == null)
            {
                throw new BusinessException("O endereço não foi encontrado.");
            }

            if(enderecoArmazenagem.Ativo == false)
            {
                throw new BusinessException("O endereço não está ativo.");
            }

            Produto produto = _unitOfWork.ProdutoRepository.GetById(requisicao.IdProduto);
            decimal pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * requisicao.Quantidade;

            //limite de peso do endereço
            if (pesoInstalacao > enderecoArmazenagem.LimitePeso)
            {
                throw new BusinessException("Quantidade ultrapassa limite de peso do endereço.");
            }

            //limite de peso vertical
        }
    }
}
