using FWLog.Data;
using FWLog.Data.Models;

namespace FWLog.Services.Services
{
    public class PrinterTypeService
    {
        private UnitOfWork _uow;

        public PrinterTypeService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void Add(PrinterType printerType)
        {
            // Adicionar validações se necessário...

            _uow.BOPrinterTypeRepository.Add(printerType);
            _uow.SaveChanges();
        }

        public void Edit(PrinterType printerType)
        {
            // Adicionar validações se necessário...

            _uow.BOPrinterTypeRepository.Update(printerType);
            _uow.SaveChanges();
        }

        public void Delete(PrinterType printerType)
        {
            // Adicionar validações se necessário...

            _uow.BOPrinterTypeRepository.Delete(printerType);
            _uow.SaveChanges();
        }
    }
}
