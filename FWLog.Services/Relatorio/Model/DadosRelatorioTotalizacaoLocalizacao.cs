
namespace FWLog.Services.Relatorio.Model
{
    public class DadosRelatorioTotalizacaoLocalizacao : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Endereço", Tamanho = 200)]
        public string CodigoEndereco { get; set; }

        [ColunaRelatorio(Nome = "Referência", Tamanho = 200)]
        public string ReferenciaProduto { get; set; }

        [ColunaRelatorio(Nome = "UN", Tamanho = 200)]
        public string Unidade { get; set; }

        [ColunaRelatorio(Nome = "Quantidade", Tamanho = 200)]
        public string Quantidade { get; set; }
    }
}