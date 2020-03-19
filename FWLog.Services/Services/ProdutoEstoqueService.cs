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

        public void AtualizarOuInserirEnderecoArmazenagem(ProdutoEstoque produtoEstoque, long? idEnderecoArmazenagem)
        {
            produtoEstoque.IdEnderecoArmazenagem = idEnderecoArmazenagem;
            _unitOfWork.ProdutoEstoqueRepository.Update(produtoEstoque);
            _unitOfWork.SaveChanges();
        }
    }
}
