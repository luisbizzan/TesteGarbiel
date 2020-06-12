using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class LoteMovimentacaoListaFiltro
    {
        public long IdEmpresa { get; set; }

        public long? IdLote { get; set; }

        public long? IdProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public DateTime? DataHoraInicial { get; set; }

        public DateTime? DataHoraFinal { get; set; }

        public string IdUsuarioMovimentacao { get; set; }
        
        public long? IdEnderecoArmazenagem { get; set; }

        public long? IdPontoArmazenagem { get; set; }

        public long? IdNivelArmazenagem { get; set; }

        public int? IdLoteMovimentacaoTipo { get; set; }
    }
}
