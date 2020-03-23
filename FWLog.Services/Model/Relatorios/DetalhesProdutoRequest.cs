namespace FWLog.Services.Model.Relatorios
{
    public class DetalhesProdutoRequest
    {
        public long IdEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public long IdProduto { get; set; }
        public string EnderecoImagem { get; set; }
    }
}
