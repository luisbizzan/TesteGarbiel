using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using System;
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

        public void Error(ApplicationEnum application, Exception ex)
        {
            var applicationLog = new ApplicationLog
            {
                Created = DateTime.Now,
                IdApplication = application.GetHashCode(),
                Level = ApplicationLogLevel.Error.Value,
                Exception = string.Format("{0} - {1} ", ex.Message, ex.StackTrace),
                Message = ex.Message
            };

            Add(applicationLog);
        }

        public void Warn(ApplicationEnum application, Exception ex)
        {
            var applicationLog = new ApplicationLog
            {
                Created = DateTime.Now,
                IdApplication = application.GetHashCode(),
                Level = ApplicationLogLevel.Warn.Value,
                Exception = string.Format("{0} - {1} ", ex.Message, ex.StackTrace),
                Message = ex.Message
            };

            Add(applicationLog);
        }
    }
}
