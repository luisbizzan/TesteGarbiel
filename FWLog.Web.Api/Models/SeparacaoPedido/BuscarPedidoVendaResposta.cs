using System.Collections.Generic;

namespace FWLog.Web.Api.Models.SeparacaoPedido
{
    public class BuscarPedidoVendaResposta
    {
        public long IdPedidoVenda { get; set; }
        public string NroPedidoVenda { get; set; }
        public long IdEmpresa { get; set; }
        public int StatusSeparacao { get; set; }
        public List<EnderecoProduto> EnderecoProduto { get; set; }
    }

    public class EnderecoProduto
    {
        public int Corredor { get; set; }
        public string Codigo { get; set; }
        public string PontoArmazenagem { get; set; }
        public string ReferenciaProduto { get; set; }
        public int MultiploProduto { get; set; }
        public int QtdePedido { get; set; }
    }
}