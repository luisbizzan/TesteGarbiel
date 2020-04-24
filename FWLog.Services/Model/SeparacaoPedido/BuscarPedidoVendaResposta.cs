using FWLog.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class BuscarPedidoVendaResposta
    {
        public long IdPedidoVenda { get; set; }
        public int NroPedidoVenda { get; set; }
        public long IdEmpresa { get; set; }
        public int StatusSeparacao { get; set; }
        public List<GrupoCorredorArmazenagem> GrupoCorredorArmazenagem{ get; set; }
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
