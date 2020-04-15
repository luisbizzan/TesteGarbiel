namespace FWLog.Services.Relatorio.Model
{
    public class RelatorioLogisticaCorredor : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Endereço", Tamanho = 60)]
        public string Codigo { get; set; }
        [ColunaRelatorio(Nome = "Referência", Tamanho = 60)]
        public string Referencia { get; set; }
        [ColunaRelatorio(Nome = "Descrição", Tamanho = 180)]
        public string Descricao { get; set; }
        [ColunaRelatorio(Nome = "Unidade", Tamanho = 50)]
        public string Unidade { get; set; }
        [ColunaRelatorio(Nome = "Comprimento", Tamanho = 80)]
        public string Comprimento { get; set; }
        [ColunaRelatorio(Nome = "Largura", Tamanho = 55)]
        public string Largura { get; set; }
        [ColunaRelatorio(Nome = "Altura", Tamanho = 40)]
        public string Altura { get; set; }
        [ColunaRelatorio(Nome = "Cubagem", Tamanho = 50)]
        public string Cubagem { get; set; }
        [ColunaRelatorio(Nome = "Giro 6m", Tamanho = 30)]
        public string Giro6m { get; set; }
        [ColunaRelatorio(Nome = "Giro DD", Tamanho = 30)]
        public string GiroDD { get; set; }
        [ColunaRelatorio(Nome = "It.loc", Tamanho = 30)]
        public string ItLoc { get; set; }
        [ColunaRelatorio(Nome = "Saldo", Tamanho = 30)]
        public string Saldo { get; set; }
        [ColunaRelatorio(Nome = "Dura DD", Tamanho = 30)]
        public string DuraDD { get; set; }
        [ColunaRelatorio(Nome = "Dt.repo", Tamanho = 30)]
        public string DtRepo { get; set; }
    }
}
