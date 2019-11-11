namespace FWLog.Services.Relatorio.Model
{
    public class RecebimentoNotas : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Lote", Tamanho = 50)]
        public string Lote { get; set; }
        [ColunaRelatorio(Nome = "Nota", Tamanho = 50)]
        public string Nota { get; set; }
        [ColunaRelatorio(Nome = "Qtde. Peças", Tamanho = 80)]
        public int QuantidadePeças { get; set; }
        [ColunaRelatorio(Nome = "Qtde. Volumes", Tamanho = 90)]
        public int QauntidadeVolumes { get; set; }
        [ColunaRelatorio(Nome = "Atraso", Tamanho = 50)]
        public string Atraso { get; set; }
        [ColunaRelatorio(Nome = "Prazo", Tamanho = 50)]
        public string Prazo { get; set; }
        [ColunaRelatorio(Nome = "Fornecedor", Tamanho = 100)]
        public string Fornecedor { get; set; }
        [ColunaRelatorio(Nome = "Status", Tamanho = 80)]
        public string Status { get; set; }
    }
}
