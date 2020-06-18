using System.Collections.Generic;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class GerenciarVolumeRequisicao
    {
        public long? IdPedidoVendaVolume { get; set; }

        public long IdGrupoCorredorArmazenagem { get; set; }

        public string NroPedido { get; set; }

        public List<GerenciarVolumeItemViewModel> ProdutosVolumes { get; set; } = new List<GerenciarVolumeItemViewModel>();
    }
}