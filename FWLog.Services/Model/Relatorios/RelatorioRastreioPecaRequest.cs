using FWLog.Data.Models.FilterCtx;
using System;

namespace FWLog.Services.Model.Relatorios
{
    public class RelatorioRastreioPecaRequest : IRelatorioRastreioPecaListaFiltro
    {
        public long IdEmpresa { get; set; }
        public string NomeUsuario { get; set; }
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
