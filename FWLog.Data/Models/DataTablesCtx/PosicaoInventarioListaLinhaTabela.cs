using System.Collections.Generic;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class PosicaoInventarioListaLinhaTabela
    {
        public string Referencia { get; set; }

        public string DescricaoProduto { get; set; }

        public long IdLote { get; set; }

        public string Codigo { get; set; }

        public int QuantidadeProdutoPorEndereco { get; set; }

        public string SaldoTotal { get; set; }

        public long IdProduto { get; set; }
    }
}
