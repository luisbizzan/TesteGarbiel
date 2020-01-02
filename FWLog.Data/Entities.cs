using FWLog.Data.Logging;
using FWLog.Data.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FWLog.Data
{
    public class Entities : DbContext
    {
        public Entities() : base("name=Entities") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("DART");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<ApplicationLanguage> ApplicationLanguage { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<BOLogSystem> BOLogSystem { get; set; }
        public virtual DbSet<ApplicationSession> ApplicationSession { get; set; }
        public virtual DbSet<ApplicationLog> ApplicationLog { get; set; }
        public virtual DbSet<Empresa> Empresa { get; set; }
        public virtual DbSet<UsuarioEmpresa> UsuarioEmpresa { get; set; }
        public virtual DbSet<Printer> Printer { get; set; }
        public virtual DbSet<PrinterType> PrinterType { get; set; }
        public virtual DbSet<PerfilUsuario> PerfilUsuario { get; set; }
        public virtual DbSet<Fornecedor> Fornecedor { get; set; }
        public virtual DbSet<FreteTipo> FreteTipo { get; set; }
        public virtual DbSet<Lote> Lote { get; set; }
        public virtual DbSet<LoteStatus> LoteStatus { get; set; }
        public virtual DbSet<NotaFiscal> NotaFiscal { get; set; }
        public virtual DbSet<NotaFiscalItem> NotaFiscalItem { get; set; }
        public virtual DbSet<Produto> Produto { get; set; }
        public virtual DbSet<Transportadora> Transportadora { get; set; }
        public virtual DbSet<UnidadeMedida> UnidadeMedida { get; set; }
        public virtual DbSet<Quarentena> Quarentena { get; set; }
        public virtual DbSet<QuarentenaStatus> QuarentenaStatus { get; set; }
        public virtual DbSet<NotaFiscalStatus> NotaFiscalStatus { get; set; }
        public virtual DbSet<NivelArmazenagem> NivelArmazenagem { get; set; }
        public virtual DbSet<PontoArmazenagem> PontoArmazenagem { get; set; }
        public virtual DbSet<TipoMovimentacao> TipoMovimentacao { get; set; }
        public virtual DbSet<TipoArmazenagem> TipoArmazenagem { get; set; }
        public virtual DbSet<EnderecoArmazenagem> EnderecoArmazenagem { get; set; }
        public virtual DbSet<EmpresaConfig> EmpresaConfig { get; set; }
        public virtual DbSet<TipoConferencia> TipoConferencia { get; set; }
        public virtual DbSet<EmpresaTipo> EmpresaTipo { get; set; }
        public virtual DbSet<LoteDivergencia> LoteDivergencia { get; set; }
        public virtual DbSet<LoteDivergenciaStatus> LoteDivergenciaStatus { get; set; }
        public virtual DbSet<LoteConferencia> LoteConferencia { get; set; }
        public virtual DbSet<LogEtiquetagem> LogEtiquetagem { get; set; }
        public virtual DbSet<ProdutoEndereco> ProdutoEndereco { get; set; }
        public virtual DbSet<ProdutoEstoque> ProdutoEstoque { get; set; }
        public virtual DbSet<QuarentenaHistorico> QuarentenaHistorico { get; set; }

        public IAuditLog AuditLog { get; private set; }

        public Entities(IAuditLog auditLog)
        {
            AuditLog = auditLog;
        }

        public override int SaveChanges()
        {
            AuditLog.AddLogsToContextAndSaveChanges(this, out int nonLogChanges);

            return nonLogChanges;
        }

        public int SaveChangesWithoutLog()
        {
            return base.SaveChanges();
        }
    }
}