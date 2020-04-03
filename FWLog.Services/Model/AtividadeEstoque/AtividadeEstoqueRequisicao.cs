namespace FWLog.Services.Model.AtividadeEstoque
{
    public class AtividadeEstoqueRequisicao
    {
        public long IdAtividadeEstoque { get; set; }

        public int QuantidadeInicial { get; set; }

        public int QuantidadeFinal { get; set; }

        public string IdUsuarioExecucao { get; set; }
    }
}
