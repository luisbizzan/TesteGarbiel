using FWLog.Data;
using FWLog.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace FWLog.Services.Services
{
    public class PerfilImpressoraService
    {
        private readonly UnitOfWork _uow;

        public PerfilImpressoraService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void Add(PerfilImpressora perfilImpressora)
        {
            _uow.PerfilImpressoraRepository.Add(perfilImpressora);
            _uow.SaveChanges();
        }

        public void Edit(PerfilImpressora perfilImpressora)
        {
            var perfilItensOld = _uow.PerfilImpressoraItemRepository.ObterPorIdPerfilImpressora(perfilImpressora.IdPerfilImpressora);

            List<PerfilImpressoraItem> perfilItensAdd =
                perfilImpressora.PerfilImpressoraItens.Where(x => !perfilItensOld.Any(y => y.IdImpressora == x.IdImpressora && y.IdImpressaoItem == x.IdImpressaoItem)).ToList();

            List<PerfilImpressoraItem> perfilItensRem =
                perfilItensOld.Where(x => !perfilImpressora.PerfilImpressoraItens.Any(y => y.IdImpressora == x.IdImpressora && y.IdImpressaoItem == x.IdImpressaoItem)).ToList();

            using (TransactionScope transactionScope = _uow.CreateTransactionScope())
            {
                _uow.PerfilImpressoraRepository.Update(perfilImpressora);
                _uow.PerfilImpressoraItemRepository.AddRange(perfilItensAdd);
                _uow.PerfilImpressoraItemRepository.Delete(perfilItensRem);

                _uow.SaveChanges();
                transactionScope.Complete();
            }
        }

        public void Delete(PerfilImpressora perfilImpressora)
        {
            using (TransactionScope transactionScope = _uow.CreateTransactionScope())
            {
                _uow.PerfilImpressoraItemRepository.Delete(perfilImpressora.PerfilImpressoraItens.ToList());
                _uow.PerfilImpressoraRepository.Delete(perfilImpressora);
                _uow.SaveChanges();
                transactionScope.Complete();
            }
        }
    }
}