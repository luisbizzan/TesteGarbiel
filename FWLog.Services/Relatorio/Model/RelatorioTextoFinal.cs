namespace FWLog.Services.Relatorio.Model
{
    public class RelatorioTextoFinal
    {
        [ColunaRelatorio(Tamanho = 800)]
        public string Texto { get; set; }
    }
}