namespace FWLog.Services.Model.AtividadeEstoque
{
    public class AtividadeEstoqueLista
    {
        public long IdEmpresa { get; set; }

        public long IdEnderecoArmazenagem { get; set; }

        public long IdProduto { get; set; }

        public int Quantidade { get; set; }

        public int? EstoqueMinimo { get; set; }
    }
}
