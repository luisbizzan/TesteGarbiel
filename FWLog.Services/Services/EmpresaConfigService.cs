using FWLog.Data;
using FWLog.Data.Models;

namespace FWLog.Services.Services
{
    public class EmpresaConfigService
    {
        private UnitOfWork _uow;

        public EmpresaConfigService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void Save(EmpresaConfig empresaConfig)
        {
            _uow.EmpresaConfigRepository.Update(empresaConfig);

            _uow.SaveChanges();
        }
    }
}
