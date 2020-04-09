
namespace FWLog.Services.Relatorio.Model
{
    public class RelatorioHistoricoColetor : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Usuário", Tamanho = 180)]
        public string Usuario { get; set; }
        [ColunaRelatorio(Nome = "Descrição", Tamanho = 330)]
        public string Descricao { get; set; }
        [ColunaRelatorio(Nome = "Aplicação", Tamanho = 100)]
        public string Aplicacao { get; set; }
        [ColunaRelatorio(Nome = "Tipo Histórico", Tamanho = 100)]
        public string Tipo { get; set; }
        [ColunaRelatorio(Nome = "Data", Tamanho = 100)]
        public string Data { get; set; }
    }
}
