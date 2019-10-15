using FWLog.Data.Logging;
using FWLog.Data.Repository.BackofficeCtx;
using FWLog.Data.Repository.GeneralCtx;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FWLog.Data
{
    public class UnitOfWork : IDisposable
    {
        private bool _disposed = false;
        private Entities _context;

        private BOLogSystemRepository _boLogSystemRepository;
        private ApplicationLanguageRepository _applicationLanguageRepository;
        private ApplicationLogRepository _applicationLogRepository;
        private ApplicationSessionRepository _applicationSessionRepository;

        public BOLogSystemRepository BOLogSystemRepository
        {
            get => _boLogSystemRepository ?? (_boLogSystemRepository = new BOLogSystemRepository(_context));
        }

        public ApplicationLanguageRepository ApplicationLanguageRepository
        {
            get => _applicationLanguageRepository ?? (_applicationLanguageRepository = new ApplicationLanguageRepository(_context));
        }

        public ApplicationLogRepository ApplicationLogRepository
        {
            get => _applicationLogRepository ?? (_applicationLogRepository = new ApplicationLogRepository(_context));
        }

        public ApplicationSessionRepository ApplicationSessionRepository
        {
            get => _applicationSessionRepository ?? (_applicationSessionRepository = new ApplicationSessionRepository(_context));
        }

        public TransactionScope CreateTransactionScope()
        {
            return CreateTransactionScope(IsolationLevel.ReadCommitted);
        }

        public TransactionScope CreateTransactionScope(IsolationLevel isolationLevel)
        {
            return CreateTransactionScope(isolationLevel, null);
        }

        public TransactionScope CreateTransactionScope(IsolationLevel isolationLevel, TimeSpan? timeout)
        {
            var opts = new TransactionOptions
            {
                IsolationLevel = isolationLevel
            };

            if (timeout == null)
            {
                int seconds = int.Parse(ConfigurationManager.AppSettings["TransactionScopeTimeout"]);
                timeout = new TimeSpan(0, 0, seconds);
            }

            opts.Timeout = timeout.Value;

            return new TransactionScope(TransactionScopeOption.Required, opts);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public int CommitWithoutLog()
        {
            return _context.SaveChangesWithoutLog();
        }

        public UnitOfWork(IAuditLog auditLog)
        {
            _context = new Entities(auditLog);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
