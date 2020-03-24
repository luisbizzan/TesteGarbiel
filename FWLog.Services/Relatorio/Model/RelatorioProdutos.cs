namespace FWLog.Services.Relatorio.Model
{
    public class RelatorioProdutos : IFwRelatorioDados
    {
        [ColunaRelatorio(Nome = "Referência", Tamanho = 150)]
        public string Referencia { get; set; }
        [ColunaRelatorio(Nome = "Descrição", Tamanho = 140)]
        public string Descricao { get; set; }
        [ColunaRelatorio(Nome = "Peso", Tamanho = 60)]
        public string Peso { get; set; }
        [ColunaRelatorio(Nome = "Largura/Altura/Comprimento", Tamanho = 160)]
        public string LarguraAlturaComprimento { get; set; }
        [ColunaRelatorio(Nome = "Unidade", Tamanho = 60)]
        public string UnidadeMedida { get; set; }
        [ColunaRelatorio(Nome = "Múltiplo", Tamanho = 80)]
        public string Multiplo { get; set; }
        [ColunaRelatorio(Nome = "Endereço", Tamanho = 80)]
        public string Endereco { get; set; }
        [ColunaRelatorio(Nome = "Status", Tamanho = 80)]
        public string Status { get; set; }
    }
}
