namespace FWLog.Web.Api.Models.Produto
{
    public class PesquisarPorCodigoBarrasReferenciaResposta
    {
        public long IdProduto { get; set; }
        public long IdUnidadeMedida { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
        public string CodigoBarras { get; set; }
        public string CodigoBarras2 { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal? Largura { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Comprimento { get; set; }
        public decimal MultiploVenda { get; set; }
        public bool Ativo { get; set; }
        public string CodigoEnderecoPicking { get; internal set; }
    }
}