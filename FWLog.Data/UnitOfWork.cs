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
        private ProdutoEstoqueRepository _produtoEstoqueRepository;
        private QuarentenaHistoricoRepository _quarentenaHistoricoRepository;
        private ImpressaoItemRepository _impressaoItemRepository;
        private PerfilImpressoraItemRepository _perfilImpressoraItemRepository;
        private PerfilImpressoraRepository _perfilImpressoraRepository;
        private TipoEtiquetagemRepository _tipoEtiquetagemRepository;
        private LoteProdutoRepository _loteProdutoRepository;
        private IntegracaoTipoRepository _integracaoTipoRepository;
        private IntegracaoEntidadeRepository _integracaoEntidadeRepository;
        private LoteMovimentacaoRepository _loteMovimentacaoRepository;
        private LoteMovimentacaoTipoRepository _loteMovimentacaoTipoRepository;
        private ClienteRepository _clienteRepository;
        private GarantiaRepository _garantiaRepository;
        private GarantiaConferenciaTipoRepository _garantiaConferenciaTipoRepository;
        private GarantiaProdutoRepository _garantiaProdutoRepository;
        private GarantiaQuarentenaRepository _garantiaQuarentenaRepository;
        private GarantiaQuarentenaHisRepository _garantiaQuarentenaHisRepository;
        private GarantiaQuarentenaStatusRepository _garantiaQuarentenaStatusRepository;
        private GarantiaStatusRepository _garantiaStatusRepository;
        private MotivoLaudoRepository _motivoLaudoRepository;
        private RepresentanteRepository _representanteRepository;
        private LoteProdutoEnderecoRepository _loteProdutoEnderecoRepository;
        private IntegracaoLogRepository _integracaoLogRepository;
        private NotaFiscalRecebimentoRepository _notaFiscalRecebimentoRepository;
        private NotaRecebimentoStatusRepository _notaRecebimentoStatusRepository;
        private ColetorAplicacaoRepository _coletorAplicacaoRepository;
        private ColetorHistoricoRepository _coletorHistoricoRepository;
        private ColetorHistoricoTipoRepository _coletorHistoricoTipoRepository;
        private AtividadeEstoqueRepository _atividadeEstoqueRepository;
        private AtividadeEstoqueTipoRepository _atividadeEstoqueTipoRepository;
        private PedidoVendaRepository _pedidoVendaRepository;
        private PedidoVendaStatusRepository _pedidoVendaStatusRepository;
        private PedidoVendaProdutoRepository _pedidoVendaProdutoRepository;
        private PedidoVendaProdutoStatusRepository _pedidoVendaProdutoStatusRepository;
        private CaixaTipoRepository _caixaTipoRepository;
        private CaixaRepository _caixaRepository;
        private GrupoCorredorArmazenagemRepository _grupoCorredorArmazenagemRepository;
        private PedidoRepository _pedidoRepository;
        private PedidoItemRepository _pedidoItemRepository;
        private PedidoVendaVolumeRepository _pedidoVendaVolumeRepository;

        public UnitOfWork()
        {
            _context = new Entities();
        }

        public PedidoVendaVolumeRepository PedidoVendaVolumeRepository
        {
            get => _pedidoVendaVolumeRepository ?? (_pedidoVendaVolumeRepository = new PedidoVendaVolumeRepository(_context));
        }

        public GrupoCorredorArmazenagemRepository GrupoCorredorArmazenagemRepository
        {
            get => _grupoCorredorArmazenagemRepository ?? (_grupoCorredorArmazenagemRepository = new GrupoCorredorArmazenagemRepository(_context));
        }

        public PedidoRepository PedidoRepository
        {
            get => _pedidoRepository ?? (_pedidoRepository = new PedidoRepository(_context));
        }

        public PedidoItemRepository PedidoItemRepository
        {
            get => _pedidoItemRepository ?? (_pedidoItemRepository = new PedidoItemRepository(_context));
        }

        public PedidoVendaProdutoStatusRepository PedidoVendaProdutoStatusRepository
        {
            get => _pedidoVendaProdutoStatusRepository ?? (_pedidoVendaProdutoStatusRepository = new PedidoVendaProdutoStatusRepository(_context));
        }

        public PedidoVendaProdutoRepository PedidoVendaProdutoRepository
        {
            get => _pedidoVendaProdutoRepository ?? (_pedidoVendaProdutoRepository = new PedidoVendaProdutoRepository(_context));
        }

        public PedidoVendaStatusRepository PedidoVendaStatusRepository
        {
            get => _pedidoVendaStatusRepository ?? (_pedidoVendaStatusRepository = new PedidoVendaStatusRepository(_context));
        }

        public PedidoVendaRepository PedidoVendaRepository
        {
            get => _pedidoVendaRepository ?? (_pedidoVendaRepository = new PedidoVendaRepository(_context));
        }

        public AtividadeEstoqueTipoRepository AtividadeEstoqueTipoRepository
        {
            get => _atividadeEstoqueTipoRepository ?? (_atividadeEstoqueTipoRepository = new AtividadeEstoqueTipoRepository(_context));
        }

        public AtividadeEstoqueRepository AtividadeEstoqueRepository
        {
            get => _atividadeEstoqueRepository ?? (_atividadeEstoqueRepository = new AtividadeEstoqueRepository(_context));
        }

        public LoteProdutoEnderecoRepository LoteProdutoEnderecoRepository
        {
            get => _loteProdutoEnderecoRepository ?? (_loteProdutoEnderecoRepository = new LoteProdutoEnderecoRepository(_context));
        }

        public LoteMovimentacaoTipoRepository LoteMovimentacaoTipoRepository
        {
            get => _loteMovimentacaoTipoRepository ?? (_loteMovimentacaoTipoRepository = new LoteMovimentacaoTipoRepository(_context));
        }

        public LoteMovimentacaoRepository LoteMovimentacaoRepository
        {
            get => _loteMovimentacaoRepository ?? (_loteMovimentacaoRepository = new LoteMovimentacaoRepository(_context));
        }

        public TipoEtiquetagemRepository TipoEtiquetagemRepository
        {
            get => _tipoEtiquetagemRepository ?? (_tipoEtiquetagemRepository = new TipoEtiquetagemRepository(_context));
        }

        public IntegracaoLogRepository IntegracaoLogRepository
        {
            get => _integracaoLogRepository ?? (_integracaoLogRepository = new IntegracaoLogRepository(_context));
        }

        public IntegracaoTipoRepository IntegracaoTipoRepository
        {
            get => _integracaoTipoRepository ?? (_integracaoTipoRepository = new IntegracaoTipoRepository(_context));
        }

        public IntegracaoEntidadeRepository IntegracaoEntidadeRepository
        {
            get => _integracaoEntidadeRepository ?? (_integracaoEntidadeRepository = new IntegracaoEntidadeRepository(_context));
        }

        public PerfilImpressoraRepository PerfilImpressoraRepository
        {
            get => _perfilImpressoraRepository ?? (_perfilImpressoraRepository = new PerfilImpressoraRepository(_context));
        }

        public PerfilImpressoraItemRepository PerfilImpressoraItemRepository
        {
            get => _perfilImpressoraItemRepository ?? (_perfilImpressoraItemRepository = new PerfilImpressoraItemRepository(_context));
        }

        public ImpressaoItemRepository ImpressaoItemRepository
        {
            get => _impressaoItemRepository ?? (_impressaoItemRepository = new ImpressaoItemRepository(_context));
        }

        public QuarentenaHistoricoRepository QuarentenaHistoricoRepository
        {
            get => _quarentenaHistoricoRepository ?? (_quarentenaHistoricoRepository = new QuarentenaHistoricoRepository(_context));
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

        public LoteProdutoRepository LoteProdutoRepository
        {
            get => _loteProdutoRepository ?? (_loteProdutoRepository = new LoteProdutoRepository(_context));
        }

        public ClienteRepository ClienteRepository
        {
            get => _clienteRepository ?? (_clienteRepository = new ClienteRepository(_context));
        }

        public GarantiaRepository GarantiaRepository
        {
            get => _garantiaRepository ?? (_garantiaRepository = new GarantiaRepository(_context));
        }

        public GarantiaConferenciaTipoRepository GarantiaConferenciaTipoRepository
        {
            get => _garantiaConferenciaTipoRepository ?? (_garantiaConferenciaTipoRepository = new GarantiaConferenciaTipoRepository(_context));
        }

        public GarantiaProdutoRepository GarantiaProdutoRepository
        {
            get => _garantiaProdutoRepository ?? (_garantiaProdutoRepository = new GarantiaProdutoRepository(_context));
        }

        public GarantiaQuarentenaRepository GarantiaQuarentenaRepository
        {
            get => _garantiaQuarentenaRepository ?? (_garantiaQuarentenaRepository = new GarantiaQuarentenaRepository(_context));
        }

        public GarantiaQuarentenaHisRepository GarantiaQuarentenaHisRepository
        {
            get => _garantiaQuarentenaHisRepository ?? (_garantiaQuarentenaHisRepository = new GarantiaQuarentenaHisRepository(_context));
        }

        public GarantiaQuarentenaStatusRepository GarantiaQuarentenaStatusRepository
        {
            get => _garantiaQuarentenaStatusRepository ?? (_garantiaQuarentenaStatusRepository = new GarantiaQuarentenaStatusRepository(_context));
        }

        public GarantiaStatusRepository GarantiaStatusRepository
        {
            get => _garantiaStatusRepository ?? (_garantiaStatusRepository = new GarantiaStatusRepository(_context));
        }

        public MotivoLaudoRepository MotivoLaudoRepository
        {
            get => _motivoLaudoRepository ?? (_motivoLaudoRepository = new MotivoLaudoRepository(_context));
        }

        public RepresentanteRepository RepresentanteRepository
        {
            get => _representanteRepository ?? (_representanteRepository = new RepresentanteRepository(_context));
        }

        public NotaFiscalRecebimentoRepository NotaFiscalRecebimentoRepository
        {
            get => _notaFiscalRecebimentoRepository ?? (_notaFiscalRecebimentoRepository = new NotaFiscalRecebimentoRepository(_context));
        }

        public NotaRecebimentoStatusRepository NotaRecebimentoStatusRepository
        {
            get => _notaRecebimentoStatusRepository ?? (_notaRecebimentoStatusRepository = new NotaRecebimentoStatusRepository(_context));
        }

        public ColetorAplicacaoRepository ColetorAplicacaoRepository
        {
            get => _coletorAplicacaoRepository ?? (_coletorAplicacaoRepository = new ColetorAplicacaoRepository(_context));
        }

        public ColetorHistoricoRepository ColetorHistoricoRepository
        {
            get => _coletorHistoricoRepository ?? (_coletorHistoricoRepository = new ColetorHistoricoRepository(_context));
        }

        public ColetorHistoricoTipoRepository ColetorHistoricoTipoRepository
        {
            get => _coletorHistoricoTipoRepository ?? (_coletorHistoricoTipoRepository = new ColetorHistoricoTipoRepository(_context));
        }

        public CaixaTipoRepository CaixaTipoRepository
        {
            get => _caixaTipoRepository ?? (_caixaTipoRepository = new CaixaTipoRepository(_context));
        }

        public CaixaRepository CaixaRepository
        {
            get => _caixaRepository ?? (_caixaRepository = new CaixaRepository(_context));
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