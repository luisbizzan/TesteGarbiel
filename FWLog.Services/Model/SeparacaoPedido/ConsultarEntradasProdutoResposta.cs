using System;
using System.Collections.Generic;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class ConsultarEntradasProdutoResposta
    {
        public long IdProduto { get; set; }

        public string ReferenciaProduto { get; set; }

        public List<ConsultarEntradasProdutoEntradaResposta> ListaEntradas { get; set; }
    }

    public class ConsultarEntradasProdutoEntradaResposta
    {
        public DateTime DataEntrada { get; set; }

        public int QuantidadeEntrada { get; set; }
    }
}