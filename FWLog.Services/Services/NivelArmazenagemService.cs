using FWLog.Data;
using FWLog.Data.Models;

namespace FWLog.Services.Services
{
    public class NivelArmazenagemService
    {
        private readonly UnitOfWork _uow;

        public NivelArmazenagemService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void Add(NivelArmazenagem nivelArmazenagem)
        {
            // Adicionar validações se necessário...

            _uow.NivelArmazenagemRepository.Add(nivelArmazenagem);
            _uow.SaveChanges();
        }

        public void Edit(NivelArmazenagem nivelArmazenagem)
        {
            // Adicionar validações se necessário...

            _uow.NivelArmazenagemRepository.Update(nivelArmazenagem);
            _uow.SaveChanges();
        }

        public void Delete(NivelArmazenagem nivelArmazenagem)
        {
            // Adicionar validações se necessário...

            _uow.NivelArmazenagemRepository.Delete(nivelArmazenagem);
            _uow.SaveChanges();
        }
    }
}
