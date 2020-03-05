using FWLog.Data;
using FWLog.Data.Models;

namespace FWLog.Services.Services
{
    public class GarantiaService
    {
        UnitOfWork _uow;

        public GarantiaService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void Add(Garantia garantia)
        {
            // Adicionar validações se necessário...

            _uow.GarantiaRepository.Add(garantia);
            _uow.SaveChanges();
        }

        public void Edit(Garantia garantia)
        {
            // Adicionar validações se necessário...

            _uow.GarantiaRepository.Update(garantia);
            _uow.SaveChanges();
        }

        public void Delete(Garantia garantia)
        {
            // Adicionar validações se necessário...

            _uow.GarantiaRepository.Delete(garantia);
            _uow.SaveChanges();
        }
    }
}
