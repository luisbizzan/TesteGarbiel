using System;

namespace FWLog.Data.Models.FilterCtx
{
    public interface IRelatorioRastreioPecaListaFiltro
    {
        long IdEmpresa { get; set; }
        long? IdLote { get; set; }
        int? NroNota { get; set; }
        string ReferenciaPronduto { get; set; }
        DateTime? DataCompraMinima { get; set; }
        DateTime? DataCompraMaxima { get; set; }
        DateTime? DataRecebimentoMinima { get; set; }
        DateTime? DataRecebimentoMaxima { get; set; }
        long? QtdCompraMinima { get; set; }
        long? QtdCompraMaxima { get; set; }
        long? QtdRecebidaMinima { get; set; }
        long? QtdRecebidaMaxima { get; set; }
    }

    public class RelatorioRastreioPecaListaFiltro : IRelatorioRastreioPecaListaFiltro
    {
        public long IdEmpresa { get; set; }
        public long? IdLote { get; set; }
        public int? NroNota { get; set; }
        public string ReferenciaPronduto { get; set; }
        public DateTime? DataCompraMinima { get; set; }
        public DateTime? DataCompraMaxima { get; set; }
        public DateTime? DataRecebimentoMinima { get; set; }
        public DateTime? DataRecebimentoMaxima { get; set; }
        public long? QtdCompraMinima { get; set; }
        public long? QtdCompraMaxima { get; set; }
        public long? QtdRecebidaMinima { get; set; }
        public long? QtdRecebidaMaxima { get; set; }
    }
}
