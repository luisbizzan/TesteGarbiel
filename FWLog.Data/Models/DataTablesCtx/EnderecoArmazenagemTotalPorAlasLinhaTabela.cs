using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class EnderecoArmazenagemTotalPorAlasLinhaTabela
    {
        public long IdEnderecoArmazenagem { get; set; }

        public string CodigoEndereco { get; set; }

        public int Corredor { get; set; }

        public long? IdLote { get; set; }

        public string IdUsuarioInstalacao { get; set; }

        public string ReferenciaProduto { get; set; }

        public DateTime? DataInstalacao { get; set; }

        public decimal? PesoProduto { get; set; }

        public int? QuantidadeProdutoPorEndereco { get; set; }

        public decimal? PesoTotalDeProduto { get; set; }
    }
}
