using System.Collections.Generic;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class BuscarPedidoVendaResposta
    {
        public long IdPedidoVenda { get; set; }

        public string NroPedidoVenda { get; set; }

        public bool SeparacaoIniciada { get; set; }

        public string IdUsuarioSeparacaoAndamento { get; set; }

        public string UserNameSeparacaoAndamento { get; set; }

        public long IdPedidoVendaVolume { get; set; }

        public int NroVolume { get; set; }

        public long IdCaixaVolume { get; set; }

        public List<BuscarPedidoVendaGrupoCorredorResposta> ListaCorredoresSeparacao { get; set; }
    }

    public class BuscarPedidoVendaGrupoCorredorResposta
    {
        public long IdGrupoCorredorArmazenagem { get; set; }

        public int CorredorInicial { get; set; }

        public int CorredorFinal { get; set; }

        public List<BuscarPedidoVendaGrupoCorredorEnderecoProdutoResposta> ListaEnderecosArmazenagem { get; set; }
    }

    public class BuscarPedidoVendaGrupoCorredorEnderecoProdutoResposta
    {
        public long IdPontoArmazenagem { get; set; }

        public int Corredor { get; set; }

        public string Codigo { get; set; }

        public string PontoArmazenagem { get; set; }

        public long IdProduto { get; set; }

        public string ReferenciaProduto { get; set; }

        public decimal MultiploProduto { get; set; }

        public int QtdePedido { get; set; }

        public int QtdSeparada { get; set; }

        public bool Picking { get; set; }
    }
}