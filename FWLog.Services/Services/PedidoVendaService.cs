using FWLog.Data;
using log4net;

namespace FWLog.Services.Services
{
    public class PedidoVendaService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public PedidoVendaService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }
    }
}


