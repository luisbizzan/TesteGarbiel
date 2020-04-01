namespace FWLog.Services.Model.Relatorios
{
    public class ImprimriDetalhesProdutoRequest
    {
        public long IdImpressora { get; set; }
        public long IdEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public long IdProduto { get; set; }
    }
}
