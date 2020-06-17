namespace FWLog.Services.Model.Armazenagem
{
    public class ValidarQuantidadeInstalacaoRequisicao
    {
        public long IdEmpresa { get; set; }
        public long IdLote { get; set; }
        public long IdProduto { get; set; }
        public int Quantidade { get; set; }
        public bool PermissaoInstalarForaMultiplo { get; set; }
    }
}
