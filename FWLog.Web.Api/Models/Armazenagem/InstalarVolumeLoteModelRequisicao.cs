namespace FWLog.Web.Api.Models.Armazenagem
{
    public class InstalarVolumeLoteModelRequisicao
    {
        public long IdLote { get; set; }
        public long IdProduto { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeCaixas { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public string UsuarioPermissaoInstalarForaMultiplo { get; set; }
    }
}