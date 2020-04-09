namespace FWLog.Services.Model.AtividadeEstoque
{
    public class AtividadeEstoqueRequisicao
    {
        public long IdAtividadeEstoque { get; set; }

        public long IdLote { get; set; }

        public int QuantidadeInicial { get; set; }

        public int QuantidadeFinal { get; set; }
    }
}
