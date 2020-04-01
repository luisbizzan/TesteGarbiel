namespace FWLog.Web.Api.Models.Lote
{
    public class BuscaProdutoNoLoteResposta
    {
        public long IdLoteProduto { get; set; }

        public int IdEmpresa { get; set; }

        public long IdLote { get; set; }

        public long IdProduto { get; set; }

        public int QuantidadeRecebida { get; set; }

        public int Saldo { get; set; }
    }
}