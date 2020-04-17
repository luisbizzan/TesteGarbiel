using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using System;
namespace FWLog.Services.Services
{
    public class ProdutoEstoqueService : BaseService
    {
        private UnitOfWork _unitOfWork;

        public ProdutoEstoqueService(UnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public void AtualizarOuInserirEnderecoArmazenagem(long idProduto, long idEnderecoArmazenagem, long idEmpresa, string idUsuario)
        {
            var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(idProduto, idEmpresa);

            if (produtoEstoque == null)
            {
                throw new BusinessException("Produto não localizado!");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                produtoEstoque.IdEnderecoArmazenagem = idEnderecoArmazenagem;
                _unitOfWork.ProdutoEstoqueRepository.Update(produtoEstoque);
                _unitOfWork.SaveChanges();

                var update = new LoteProdutoEndereco();
                update.Quantidade = default(int);

                var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoProdutoEmpresa(idEnderecoArmazenagem, produtoEstoque.IdProduto, produtoEstoque.IdEmpresa);

                if (loteProdutoEndereco == null)
                {
                    var newLoteProdutoEndereco = new LoteProdutoEndereco();

                    newLoteProdutoEndereco.IdLoteProdutoEndereco = 0;
                    newLoteProdutoEndereco.IdEnderecoArmazenagem = idEnderecoArmazenagem;
                    newLoteProdutoEndereco.IdProduto = produtoEstoque.IdProduto;
                    newLoteProdutoEndereco.IdLote = null;
                    newLoteProdutoEndereco.Quantidade = 0;
                    newLoteProdutoEndereco.PesoTotal = 0;
                    newLoteProdutoEndereco.IdEmpresa = produtoEstoque.IdEmpresa;
                    newLoteProdutoEndereco.IdUsuarioInstalacao = idUsuario;
                    newLoteProdutoEndereco.DataHoraInstalacao = DateTime.Now;

                    _unitOfWork.LoteProdutoEnderecoRepository.Add(newLoteProdutoEndereco);
                }
                else
                {
                    loteProdutoEndereco.IdEnderecoArmazenagem = idEnderecoArmazenagem;
                    loteProdutoEndereco.DataHoraInstalacao = DateTime.Now;

                    _unitOfWork.LoteProdutoEnderecoRepository.Update(loteProdutoEndereco);
                }

                _unitOfWork.SaveChanges();

                transacao.Complete();
            }
        }
    }
}