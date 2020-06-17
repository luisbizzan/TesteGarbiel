namespace FWLog.Services.Model.Armazenagem
{
    public class ValidarEnderecoInstalacaoRequisicao
    {
        public long IdEmpresa { get; set; }
        public long IdLote { get; set; }
        public long IdProduto { get; set; }
        public int Quantidade { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public bool PermissaoInstalarForaMultiplo { get; set; }
    }
}
