namespace FWLog.Services.Relatorio.Model
{
    public class RelatorioTotalizadorFinal 
    {
        [ColunaRelatorio(Tamanho = 150)]
        public string Texto { get; set; }

        [ColunaRelatorio(Tamanho = 40)]
        public int Valor { get; set; }
    }
}