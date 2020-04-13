namespace FWLog.Services.Relatorio.Model
{
    public class RelatorioLogisticaCorredor : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Endereço", Tamanho = 80)]
        public string Codigo { get; set; }
        [ColunaRelatorio(Nome = "Referência", Tamanho = 80)]
        public string Referencia { get; set; }
        [ColunaRelatorio(Nome = "Descrição", Tamanho = 80)]
        public string Descricao { get; set; }
        [ColunaRelatorio(Nome = "Unidade", Tamanho = 80)]
        public string Unidade { get; set; }
        [ColunaRelatorio(Nome = "Comprimento", Tamanho = 80)]
        public string Comprimento { get; set; }
        [ColunaRelatorio(Nome = "Largura", Tamanho = 80)]
        public string Largura { get; set; }
        [ColunaRelatorio(Nome = "Altura", Tamanho = 80)]
        public string Altura { get; set; }
        [ColunaRelatorio(Nome = "Cubagem", Tamanho = 80)]
        public string Cubagem { get; set; }
        [ColunaRelatorio(Nome = "Giro 6m", Tamanho = 80)]
        public string Giro6m { get; set; }
        [ColunaRelatorio(Nome = "Giro DD", Tamanho = 80)]
        public string GiroDD { get; set; }
        [ColunaRelatorio(Nome = "It.loc", Tamanho = 80)]
        public string ItLoc { get; set; }
        [ColunaRelatorio(Nome = "Saldo", Tamanho = 80)]
        public string Saldo { get; set; }
        [ColunaRelatorio(Nome = "Dura DD", Tamanho = 80)]
        public string DuraDD { get; set; }
        [ColunaRelatorio(Nome = "Dt.repo", Tamanho = 80)]
        public string DtRepo { get; set; }
    }
}
