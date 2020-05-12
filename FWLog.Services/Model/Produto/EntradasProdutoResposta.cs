using System;
using System.Collections.Generic;

namespace FWLog.Services.Model.Produto
{
    public class EntradasProdutoResposta
    {
        public long IdProduto { get; set; }

        public string ReferenciaProduto { get; set; }

        public List<EntradasProdutoItemResposta> ListaEntradas { get; set; }
    }

    public class EntradasProdutoItemResposta
    {
        public DateTime DataEntrada { get; set; }

        public int QuantidadeEntrada { get; set; }
    }
}