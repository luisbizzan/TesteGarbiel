namespace FWLog.Services.Model.Produto
{
    public class ProdutoViewModel
    {
        public long IdProduto { get; set; }
        public long? IdEnderecoArmazenagem { get; set; }
        public string CodigoEnderecoArmazenagem { get; set; }
        public decimal? CubagemProduto { get; set; }
        public long IdUnidadeMedida { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
       public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal? Largura { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Comprimento { get; set; }
        public decimal? MetroCubico { get; set; }
        public decimal MultiploVenda { get; set; }
        public bool Ativo { get; set; }
        public bool IsEmbalagemFornecedor { get; set; }
        public bool IsEmbalagemFornecedorVolume { get; set; }
    }
}
