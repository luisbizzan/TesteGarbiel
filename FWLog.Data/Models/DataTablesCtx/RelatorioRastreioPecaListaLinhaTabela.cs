using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioRastreioPecaListaLinhaTabela
    {
        public long IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public LoteStatusEnum IdLoteStatus { get; set; }
        public long IdNotaFiscal { get; set; }
        public long IdProduto { get; set; }
        public long IdLote { get; set; }
        public int NroNota { get; set; }
        public string ReferenciaProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public DateTime DataCompra { get; set; }
        public DateTime DataRecebimento { get; set; }
        public int QtdCompra { get; set; }
        public int QtdRecebida { get; set; }
    }
}
