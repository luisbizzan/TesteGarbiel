using System;

namespace FWLog.Services.Relatorio.Model
{
    public class RastreioPeca : IFwRelatorioDados
    {
        //[ColunaRelatorio(Nome = "Número Empresa")]
        //public long IdEmpresa { get; set; }

        [ColunaRelatorio(Nome = "Empresa", Tamanho = 100)]
        public string Empresa { get; set; }

        [ColunaRelatorio(Nome = "Número Lote", Tamanho = 100)]
        public long IdLote { get; set; }

        [ColunaRelatorio(Nome = "Número Nota", Tamanho = 100)]
        public int NroNota { get; set; }

        [ColunaRelatorio(Nome = "Referência do Pronduto", Tamanho = 100)]
        public string ReferenciaPronduto { get; set; }

        [ColunaRelatorio(Nome = "Recebimento", Tamanho = 100)]
        public DateTime DataRecebimento { get; set; }

        [ColunaRelatorio(Nome = "Quantidade Compra", Tamanho = 100)]
        public long? QtdCompra { get; set; }

        [ColunaRelatorio(Nome = "Quantidade Recebida", Tamanho = 100)]
        public long? QtdRecebida { get; set; }
    }
}
