using FWLog.Data.Logging;
using FWLog.Data.Repository.BackofficeCtx;
using FWLog.Data.Repository.GeneralCtx;
using System;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FWLog.Data
{
    public class UnitOfWork : IDisposable
    {
        private bool _disposed = false;
        private readonly Entities _context;

        private BOLogSystemRepository _boLogSystemRepository;
        private ApplicationLanguageRepository _applicationLanguageRepository;
        private ApplicationLogRepository _applicationLogRepository;
        private ApplicationSessionRepository _applicationSessionRepository;
        private CompanyRepository _companyRepository;
        private BOPrinterRepository _boPrinterRepository;
        private BOPrinterTypeRepository _boPrinterTypeRepository;
        private PerfilUsuarioRepository _perfilUsuarioRepository;
        private UserCompanyRepository _userCompanyRepository;
        private FornecedorRepository _fornecedorRepository;
        private FreteTipoRepository _freteTipoRepository;
        private LoteRepository _loteRepository;
        private ProdutoRepository _produtoRepository;
        private TransportadoraRepository _transportadoraRepository;
        private UnidadeMedidaRepository _unidadeMedidaRepository;
        private NotaFiscalRepository _notaFiscalRepository;
        private LoteStatusRepository _loteStatusRepository;

        public NotaFiscalRepository NotaFiscalRepository
        {
            get => _notaFiscalRepository ?? (_notaFiscalRepository = new NotaFiscalRepository(_context));
        }

        public LoteStatusRepository LoteStatusRepository
        {
            get => _loteStatusRepository ?? (_loteStatusRepository = new LoteStatusRepository(_context));
        }

        public UnidadeMedidaRepository UnidadeMedidaRepository
        {
            get => _unidadeMedidaRepository ?? (_unidadeMedidaRepository = new UnidadeMedidaRepository(_context));
        }

        public TransportadoraRepository TransportadoraRepository
        {
            get => _transportadoraRepository ?? (_transportadoraRepository = new TransportadoraRepository(_context));
        }

        public ProdutoRepository ProdutoRepository
        {
            get => _produtoRepository ?? (_produtoRepository = new ProdutoRepository(_context));
        }

        public LoteRepository LoteRepository
        {
            get => _loteRepository ?? (_loteRepository = new LoteRepository(_context));
        }

        public FreteTipoRepository FreteTipoRepository
        {
            get => _freteTipoRepository ?? (_freteTipoRepository = new FreteTipoRepository(_context));
        }

        public FornecedorRepository FornecedorRepository
        {
            get => _fornecedorRepository ?? (_fornecedorRepository = new FornecedorRepository(_context));
        }

        public UserCompanyRepository UserCompanyRepository
        {
            get => _userCompanyRepository ?? (_userCompanyRepository = new UserCompanyRepository(_context));
        }

        public CompanyRepository CompanyRepository
        {
            get => _companyRepository ?? (_companyRepository = new CompanyRepository(_context));
        }

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

        public BOPrinterRepository BOPrinterRepository
        {
            get => _boPrinterRepository ?? (_boPrinterRepository = new BOPrinterRepository(_context));
        }

        public BOPrinterTypeRepository BOPrinterTypeRepository
        {
            get => _boPrinterTypeRepository ?? (_boPrinterTypeRepository = new BOPrinterTypeRepository(_context));
        }

        public PerfilUsuarioRepository PerfilUsuarioRepository
        {
            get => _perfilUsuarioRepository ?? (_perfilUsuarioRepository = new PerfilUsuarioRepository(_context));
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

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                string msg = string.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {
                    msg = string.Format("Entidade do tipo \"{0}\" no estado \"{1}\" tem os seguintes erros de validação:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    StringBuilder strb = new StringBuilder();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        strb.Append(string.Format("{2} Property: \"{0}\", Erro: \"{1}\"", ve.PropertyName, ve.ErrorMessage, Environment.NewLine));
                    }

                    msg = string.Concat(msg, strb.ToString());
                }
                throw new Exception(msg);
            }
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
