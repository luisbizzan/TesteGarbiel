namespace FWLog.Web.Api.Models.Armazenagem
{
    public class AjustarVolumeLoteModelRequisicao
    {
        public long IdLote { get; set; }
        public long IdProduto { get; set; }
        public int Quantidade { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
    }
}