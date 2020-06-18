using System.Collections.Generic;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class GerenciarVolumeRequisicao
    {
        public long? IdPedidoVendaVolume { get; set; }

        public long IdGrupoCorredorArmazenagem { get; set; }

        public string NroPedido { get; set; }

        public List<GerenciarVolumeItemRequisicao> ProdutosVolumes { get; set; } = new List<GerenciarVolumeItemRequisicao>();
    }

    public class GerenciarVolumeItemRequisicao
    {
        public long IdProduto { get; set; }

        public long IdPedidoVendaVolumeOrigem { get; set; }

        public int Quantidade { get; set; }

        public long? IdLote { get; set; }
    }
}