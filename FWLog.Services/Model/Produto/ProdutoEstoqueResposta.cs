using System.Collections.Generic;

namespace FWLog.Services.Model.Produto
{
    public class ProdutoEstoqueResposta
    {
        public long IdProduto { get; set; }
        public string Referencia { get; set; }
        public int QtdEstoque { get; set; }
        public int QtdReservada { get; set; }
        public int Saldo { get; set; }
        public List<PontoArmazenagemResposta> PontosArmazenagem { get; set; }
    }

    public class PontoArmazenagemResposta
    {
        public long IdPontoArmazenagem { get; set; }
        public string Descricao { get; set; }
        public int Saldo { get; set; }
    }
}
