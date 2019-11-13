using FWLog.Data;
using System.Transactions;

namespace FWLog.Services.Services
{
    public class ApplicationLogService
    {
        private UnitOfWork _unitOfWork;

        public ApplicationLogService(UnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public void Add(ApplicationLog applicationLog)
        {
            using (TransactionScope transactionScope = _unitOfWork.CreateTransactionScope())
            {
                _unitOfWork.ApplicationLogRepository.Add(applicationLog);

                _unitOfWork.SaveChanges();
                transactionScope.Complete();
            }
        }

    }
}
