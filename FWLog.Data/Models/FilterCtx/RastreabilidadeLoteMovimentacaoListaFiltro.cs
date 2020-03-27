using System;

namespace FWLog.Data.Models.FilterCtx
{
    public class RastreabilidadeLoteMovimentacaoListaFiltro
    {
        public long IdLote { get; set; }

        public long IdEmpresa { get; set; }

        public long IdProduto { get; set; }

        public string IdUsuarioMovimentacao { get; set; }

        public int? IdLoteMovimentacaoTipo { get; set; }

        public int? QuantidadeInicial { get; set; }

        public int? QuantidadeFinal { get; set; }

        public DateTime? DataHoraInicial { get; set; }

        public DateTime? DataHoraFinal { get; set; }
    }
}
