namespace FWLog.Web.Backoffice.Models.PedidoVendaVolumeCtx
{
    public class ConsultarDadosProdutoModelView
    {
        public long IdProduto { get; set; }
        public long? IdLote { get; set; }
        public int QuantidadeSeparar { get; set; }
        public decimal MultiploVenda { get; set; }
        public long IdGrupoCorredorArmazenagem { get; set; }
        public int CorredorInicio { get; set; }
        public int CorredorFim { get; set; }
    }
}