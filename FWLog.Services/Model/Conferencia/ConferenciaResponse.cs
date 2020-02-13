using System;

namespace FWLog.Services.Model.Conferencia
{
    public class ConferenciaResponse
    {
        public Data.Models.Lote Lote  { get; set; }
        public Data.Models.Produto Produto { get; set; }
        public Data.Models.EmpresaConfig EmpresaConfig { get; set; }
        public Data.Models.ProdutoEstoque ProdutoEstoque { get; set; }
        public string Mensagem { get; set; }
        public bool Sucesso { get; set; }
    }
}
