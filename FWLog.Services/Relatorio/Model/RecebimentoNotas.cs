namespace FWLog.Services.Relatorio.Model
{
    public class RecebimentoNotas : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Lote", Tamanho = 45)]
        public string Lote { get; set; }
        [ColunaRelatorio(Nome = "Nota", Tamanho = 60)]
        public string Nota { get; set; }
        [ColunaRelatorio(Nome = "Peças", Tamanho = 40)]
        public int QuantidadePeca { get; set; }
        [ColunaRelatorio(Nome = "Volumes", Tamanho = 50)]
        public long QauntidadeVolumes { get; set; }
        [ColunaRelatorio(Nome = "Atraso", Tamanho = 45)]
        public string Atraso { get; set; }
        [ColunaRelatorio(Nome = "Prazo", Tamanho = 60)]
        public string Prazo { get; set; }
        [ColunaRelatorio(Nome = "Fornecedor", Tamanho = 150)]
        public string Fornecedor { get; set; }
        [ColunaRelatorio(Nome = "Status", Tamanho = 80)]
        public string Status { get; set; }
    }
}
