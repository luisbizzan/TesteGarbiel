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
        private EmpresaRepository _empresaRepository;
        private BOPrinterRepository _boPrinterRepository;
        private BOPrinterTypeRepository _boPrinterTypeRepository;
        private PerfilUsuarioRepository _perfilUsuarioRepository;
        private UsuarioEmpresaRepository _usuarioEmpresaRepository;
        private FornecedorRepository _fornecedorRepository;
        private FreteTipoRepository _freteTipoRepository;
        private LoteRepository _loteRepository;
        private ProdutoRepository _produtoRepository;
        private TransportadoraRepository _transportadoraRepository;
        private UnidadeMedidaRepository _unidadeMedidaRepository;
        private NotaFiscalRepository _notaFiscalRepository;
        private LoteStatusRepository _loteStatusRepository;
        private NotaFiscalItemRepository _notaFiscalItemRepository;
        private QuarentenaRepository _quarentenaRepository;
        private QuarentenaStatusRepository _quarentenaStatusRepository;
        private NotaFiscalStatusRepository _notaFiscalStatusRepository;
		private NivelArmazenagemRepository _nivelArmazenagemRepository;
        private PontoArmazenagemRepository _pontoArmazenagemRepository;
        private TipoMovimentacaoRepository _tipoMovimentacaoRepository;
        private TipoArmazenagemRepository _tipoArmazenagemRepository;
        private EmpresaTipoRepository _empresaTipoRepository;
        private EnderecoArmazenagemRepository _enderecoArmazenagemRepository;
        private EmpresaConfigRepository _empresaConfigRepository;
        private LoteDivergenciaRepository _loteDivergenciaRepository;
        private TipoConferenciaRepository _tipoConferenciaRepository;
        private LoteConferenciaRepository _loteConferenciaRepository;
        private LogEtiquetagemRepository _loteEtiquetagemRepository;
        private ProdutoEnderecoRepository _produtoEnderecoRepository;
        private ProdutoEstoqueRepository _produtoEstoqueRepository;
        private QuarentenaHistoricoRepository _quarentenaHistoricoRepository;

        public QuarentenaHistoricoRepository QuarentenaHistoricoRepository
        {
            get => _quarentenaHistoricoRepository ?? (_quarentenaHistoricoRepository = new QuarentenaHistoricoRepository(_context));
        }

        public ProdutoEnderecoRepository ProdutoEnderecoRepository
        {
            get => _produtoEnderecoRepository ?? (_produtoEnderecoRepository = new ProdutoEnderecoRepository(_context));
        }

        public LogEtiquetagemRepository LogEtiquetagemRepository
        {
            get => _loteEtiquetagemRepository ?? (_loteEtiquetagemRepository = new LogEtiquetagemRepository(_context));
        }

        public ProdutoEstoqueRepository ProdutoEstoqueRepository
        {
            get => _produtoEstoqueRepository ?? (_produtoEstoqueRepository = new ProdutoEstoqueRepository(_context));
        }

        public LoteConferenciaRepository LoteConferenciaRepository
        {
            get => _loteConferenciaRepository ?? (_loteConferenciaRepository = new LoteConferenciaRepository(_context));
        }

        public TipoConferenciaRepository TipoConferenciaRepository
        {
            get => _tipoConferenciaRepository ?? (_tipoConferenciaRepository = new TipoConferenciaRepository(_context));
        }

        public LoteDivergenciaRepository LoteDivergenciaRepository
        {
            get => _loteDivergenciaRepository ?? (_loteDivergenciaRepository = new LoteDivergenciaRepository(_context));
        }

        public EmpresaConfigRepository EmpresaConfigRepository
        {
            get => _empresaConfigRepository ?? (_empresaConfigRepository = new EmpresaConfigRepository(_context));
        }

        public EnderecoArmazenagemRepository EnderecoArmazenagemRepository
        {
            get => _enderecoArmazenagemRepository ?? (_enderecoArmazenagemRepository = new EnderecoArmazenagemRepository(_context));
        }

        public EmpresaTipoRepository EmpresaTipoRepository
        {
            get => _empresaTipoRepository ?? (_empresaTipoRepository = new EmpresaTipoRepository(_context));
        }

        public TipoArmazenagemRepository TipoArmazenagemRepository
        {
            get => _tipoArmazenagemRepository ?? (_tipoArmazenagemRepository = new TipoArmazenagemRepository(_context));
        }

        public TipoMovimentacaoRepository TipoMovimentacaoRepository
        {
            get => _tipoMovimentacaoRepository ?? (_tipoMovimentacaoRepository = new TipoMovimentacaoRepository(_context));
		}

        public NivelArmazenagemRepository NivelArmazenagemRepository
        {
            get => _nivelArmazenagemRepository ?? (_nivelArmazenagemRepository = new NivelArmazenagemRepository(_context));
        }

        public PontoArmazenagemRepository PontoArmazenagemRepository
        {
            get => _pontoArmazenagemRepository ?? (_pontoArmazenagemRepository = new PontoArmazenagemRepository(_context));
        }

        public NotaFiscalStatusRepository NotaFiscalStatusRepository
        {
            get => _notaFiscalStatusRepository ?? (_notaFiscalStatusRepository = new NotaFiscalStatusRepository(_context));
        }

        public QuarentenaStatusRepository QuarentenaStatusRepository
        {
            get => _quarentenaStatusRepository ?? (_quarentenaStatusRepository = new QuarentenaStatusRepository(_context));
        }

        public QuarentenaRepository QuarentenaRepository
        {
            get => _quarentenaRepository ?? (_quarentenaRepository = new QuarentenaRepository(_context));
        }

        public NotaFiscalItemRepository NotaFiscalItemRepository
        {
            get => _notaFiscalItemRepository ?? (_notaFiscalItemRepository = new NotaFiscalItemRepository(_context));
        }

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

        public UsuarioEmpresaRepository UsuarioEmpresaRepository
        {
            get => _usuarioEmpresaRepository ?? (_usuarioEmpresaRepository = new UsuarioEmpresaRepository(_context));
        }

        public EmpresaRepository EmpresaRepository
        {
            get => _empresaRepository ?? (_empresaRepository = new EmpresaRepository(_context));
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
