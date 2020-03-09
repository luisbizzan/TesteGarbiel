using System;

namespace FWLog.Services.Relatorio.Model
{
    public class RastreioPeca : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Empresa", Tamanho = 150)]
        public string Empresa { get; set; }

        [ColunaRelatorio(Nome = "Número Lote", Tamanho = 80)]
        public long IdLote { get; set; }

        [ColunaRelatorio(Nome = "Número Nota", Tamanho = 80)]
        public int NroNota { get; set; }

        [ColunaRelatorio(Nome = "Referência - Descrição", Tamanho = 250)]
        public string ReferenciaDescricaoProduto { get; set; }
        
        [ColunaRelatorio(Nome = "Recebimento", Tamanho = 80)]
        public DateTime DataRecebimento { get; set; }

        [ColunaRelatorio(Nome = "Qtd. Compra", Tamanho = 80)]
        public long? QtdCompra { get; set; }

        [ColunaRelatorio(Nome = "Qtd. Recebida", Tamanho = 80)]
        public long? QtdRecebida { get; set; }
    }
}
