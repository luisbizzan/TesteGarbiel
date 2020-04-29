namespace FWLog.Services.Model.SeparacaoPedido
{
    public class SalvarSeparacaoProdutoResposta
    {
        public long IdPedidoVenda { get; set; }
        public long IdProduto { get; set; }
        public string Referencia { get; set; }
        public int QtdSeparar { get; set; }
        public int QtdSeparada { get; set; }
        public decimal? Multiplo { get; set; }
        public bool ProdutoSeparado { get; set; }
        public bool VolumeSeparado { get; set; }
    }
}