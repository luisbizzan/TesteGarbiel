
namespace FWLog.Services.Relatorio.Model
{
    public class DadosRelatorioRomaneio : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "NF", Tamanho = 70)]
        public string NumeroNotaFiscal { get; set; }

        [ColunaRelatorio(Nome = "CLIENTE", Tamanho = 280)]
        public string Cliente { get; set; }

        [ColunaRelatorio(Nome = "ENDEREÇO", Tamanho = 280)]
        public string Endereco { get; set; }

        [ColunaRelatorio(Nome = "TELEFONE", Tamanho = 90)]
        public string Telefone { get; set; }

        [ColunaRelatorio(Nome = "QT. VOL", Tamanho = 30)]
        public string QauntidadeVolumes { get; set; }

        [ColunaRelatorio(Nome = "", Tamanho = 30)]
        public string TipoFrete { get; set; }
    }
}