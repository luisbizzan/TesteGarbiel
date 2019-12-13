using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioRastreioPecaListaLinhaTabela
    {
        public long IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public long IdLote { get; set; }
        public int NroNota { get; set; }
        public string ReferenciaPronduto { get; set; }
        public DateTime DataRecebimento { get; set; }
        public long? QtdCompra { get; set; }
        public long? QtdRecebida { get; set; }
    }
}
