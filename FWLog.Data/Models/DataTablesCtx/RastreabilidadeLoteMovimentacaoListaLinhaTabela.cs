using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RastreabilidadeLoteMovimentacaoListaLinhaTabela
    {
        public long? IdProduto { get; set; }

        public long IdLote { get; set; }

        public string ReferenciaProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public string Tipo { get; set; }

        public int Quantidade { get; set; }

        public DateTime DataHora { get; set; }

        public string Endereco { get; set; }

        public string IdUsuarioMovimentacao { get; set; }
        public int? NroVolume { get; set; }
    }
}
