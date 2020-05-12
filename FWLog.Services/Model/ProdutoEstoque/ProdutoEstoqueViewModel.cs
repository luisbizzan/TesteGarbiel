using FWLog.Data.Models;

namespace FWLog.Services.Model.ProdutoEstoque
{
    public class ProdutoEstoqueViewModel
    {
        public long IdProduto { get; set; }

        public long IdEmpresa { get; set; }

        public int Saldo { get; set; }

        public long? IdEnderecoArmazenagem { get; set; }

        public ProdutoEstoqueStatusEnum IdProdutoEstoqueStatus { get; set; }

        public EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}
