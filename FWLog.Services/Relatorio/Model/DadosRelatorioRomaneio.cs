
namespace FWLog.Services.Relatorio.Model
{
    public class DadosRelatorioRomaneio : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "NF", Tamanho = 20)]
        public string NumeroNotaFiscal { get; set; }

        [ColunaRelatorio(Nome = "CLIENTE", Tamanho = 370)]
        public string Cliente { get; set; }

        [ColunaRelatorio(Nome = "ENDEREÇO", Tamanho = 370)]
        public string Endereco { get; set; }

        [ColunaRelatorio(Nome = "TELEFONE", Tamanho = 20)]
        public string Telefone { get; set; }

        [ColunaRelatorio(Nome = "QT. VOL", Tamanho = 10)]
        public string QauntidadeVolumes { get; set; }

        [ColunaRelatorio(Nome = "", Tamanho = 10)]
        public string TipoFrete { get; set; }
    }
}