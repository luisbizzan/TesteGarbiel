using FWLog.Services.Relatorio;

namespace FWLog.Services.Model.Relatorios
{
    public class DadosRelatorioRomaneioCampinasBateria : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "NF", Tamanho = 40)]
        public string NumeroNotaFiscal { get; set; }

        [ColunaRelatorio(Nome = "CLIENTE", Tamanho = 160)]
        public string Cliente { get; set; }

        [ColunaRelatorio(Nome = "ENDEREÇO", Tamanho = 150)]
        public string Endereco { get; set; }

        [ColunaRelatorio(Nome = "TELEFONE", Tamanho = 70)]
        public string Telefone { get; set; }

        [ColunaRelatorio(Nome = "QT. VOL", Tamanho = 60)]
        public string QuantidadeVolumes { get; set; }

        [ColunaRelatorio(Nome = "PESO", Tamanho = 30)]
        public string Peso { get; set; }

        [ColunaRelatorio(Nome = "", Tamanho = 30)]
        public string TipoFrete { get; set; }
    }
}
