using FWLog.Data.Models;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FWLog.Data
{
    public class Entities : DbContext
    {
        public Entities() : base("name=Entities") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(ConfigurationManager.AppSettings["DatabaseSchema"]);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<ApplicationLanguage> ApplicationLanguage { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
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
        public virtual DbSet<NotaFiscalRecebimento> NotaFiscalRecebimento { get; set; }
        public virtual DbSet<Produto> Produto { get; set; }
        public virtual DbSet<Transportadora> Transportadora { get; set; }
        public virtual DbSet<UnidadeMedida> UnidadeMedida { get; set; }
        public virtual DbSet<Quarentena> Quarentena { get; set; }
        public virtual DbSet<QuarentenaStatus> QuarentenaStatus { get; set; }
        public virtual DbSet<NotaFiscalStatus> NotaFiscalStatus { get; set; }
        public virtual DbSet<NotaRecebimentoStatus> NotaRecebimentoStatus { get; set; }
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
        public virtual DbSet<ProdutoEstoque> ProdutoEstoque { get; set; }
        public virtual DbSet<QuarentenaHistorico> QuarentenaHistorico { get; set; }
        public virtual DbSet<ImpressaoItem> ImpressaoItem { get; set; }
        public virtual DbSet<PerfilImpressora> PerfilImpressora { get; set; }
        public virtual DbSet<PerfilImpressoraItem> PerfilImpressoraItem { get; set; }
        public virtual DbSet<TipoEtiquetagem> TipoEtiquetagem { get; set; }
        public virtual DbSet<IntegracaoTipo> IntegracaoTipo { get; set; }
        public virtual DbSet<IntegracaoEntidade> IntegracaoEntidade { get; set; }
        public virtual DbSet<LoteProduto> LoteProduto { get; set; }
        public virtual DbSet<LoteMovimentacao> LoteMovimentacao { get; set; }
        public virtual DbSet<LoteMovimentacaoTipo> LoteMovimentacaoTipo { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Garantia> Garantia { get; set; }
        public virtual DbSet<GarantiaConferenciaTipo> GarantiaConferenciaTipo { get; set; }
        public virtual DbSet<GarantiaProduto> GarantiaProduto { get; set; }
        public virtual DbSet<GarantiaQuarentena> GarantiaQuarentena { get; set; }
        public virtual DbSet<GarantiaQuarentenaHis> GarantiaQuarentenaHist { get; set; }
        public virtual DbSet<GarantiaQuarentenaStatus> GarantiaQuarentenaStatus { get; set; }
        public virtual DbSet<GarantiaStatus> GarantiaStatus { get; set; }
        public virtual DbSet<MotivoLaudo> MotivoLaudo { get; set; }
        public virtual DbSet<Representante> Representante { get; set; }
        public virtual DbSet<LoteProdutoEndereco> LoteProdutoEndereco { get; set; }
        public virtual DbSet<IntegracaoLog> IntegracaoLog { get; set; }
        public virtual DbSet<ColetorAplicacao> ColetorAplicacao { get; set; }
        public virtual DbSet<ColetorHistoricoTipo> ColetorHistoricoTipo { get; set; }
        public virtual DbSet<ColetorHistorico> ColetorHistorico { get; set; }
        public virtual DbSet<AtividadeEstoque> AtividadeEstoque { get; set; }
        public virtual DbSet<AtividadeEstoqueTipo> AtividadeEstoqueTipo { get; set; }
        public virtual DbSet<PedidoVenda> PedidoVenda { get; set; }
        public virtual DbSet<PedidoVendaProduto> PedidoVendaProduto { get; set; }
        public virtual DbSet<PedidoVendaProdutoStatus> PedidoVendaProdutoStatus { get; set; }
        public virtual DbSet<PedidoVendaStatus> PedidoVendaStatus { get; set; }
        public virtual DbSet<CaixaTipo> CaixaTipo { get; set; }
        public virtual DbSet<Caixa> Caixa { get; set; }
        public virtual DbSet<GrupoCorredorArmazenagem> GrupoCorredorArmazenagem { get; set; }
        public virtual DbSet<Pedido> Pedido { get; set; }
        public virtual DbSet<PedidoItem> PedidoItem { get; set; }
        public virtual DbSet<PedidoVendaVolume> PedidoVendaVolume { get; set; }

        public int SaveChangesWithoutLog()
        {
            return base.SaveChanges();
        }
    }
}