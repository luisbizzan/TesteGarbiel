using FWLog.Data;
using FWLog.Data.Models;

namespace FWLog.Services.Services
{
    public class MotivoLaudoService
    {
        private readonly UnitOfWork _uow;

        public MotivoLaudoService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void Add(MotivoLaudo motivoLaudo)
        {
            // Adicionar validações se necessário...

            _uow.MotivoLaudoRepository.Add(motivoLaudo);
            _uow.SaveChanges();
        }

        public void Edit(MotivoLaudo motivoLaudo)
        {
            // Adicionar validações se necessário...

            _uow.MotivoLaudoRepository.Update(motivoLaudo);
            _uow.SaveChanges();
        }

        public void Delete(MotivoLaudo motivoLaudo)
        {
            // Adicionar validações se necessário...

            _uow.MotivoLaudoRepository.Delete(motivoLaudo);
            _uow.SaveChanges();
        }
    }
}
