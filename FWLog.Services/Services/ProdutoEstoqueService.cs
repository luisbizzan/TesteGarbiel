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

        public void AtualizarOuInserirEnderecoArmazenagem(ProdutoEstoque produtoEstoque, long? idEnderecoArmazenagem, string idUsuario)
        {
            produtoEstoque.IdEnderecoArmazenagem = idEnderecoArmazenagem;
            _unitOfWork.ProdutoEstoqueRepository.Update(produtoEstoque);

            var update = new LoteProdutoEndereco();
            update.Quantidade = default(int);
            
            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoProdutoEmpresa(idEnderecoArmazenagem.Value, produtoEstoque.IdProduto, produtoEstoque.IdEmpresa);
            
            if (loteProdutoEndereco == null)
            {
                var newLoteProdutoEndereco = new LoteProdutoEndereco();
                newLoteProdutoEndereco.IdLoteProdutoEndereco = 0;
                newLoteProdutoEndereco.IdEnderecoArmazenagem = idEnderecoArmazenagem.Value;
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
                loteProdutoEndereco.IdEnderecoArmazenagem = idEnderecoArmazenagem.Value;
                loteProdutoEndereco.DataHoraInstalacao = DateTime.Now;
                _unitOfWork.LoteProdutoEnderecoRepository.Update(loteProdutoEndereco);
            }

            

            _unitOfWork.SaveChanges();
        }
    }
}
