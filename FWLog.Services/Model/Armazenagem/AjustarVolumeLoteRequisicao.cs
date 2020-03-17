namespace FWLog.Services.Model.Armazenagem
{
    public class AjustarVolumeLoteRequisicao
    {
        public long IdLote { get; set; }
        public long IdProduto { get; set; }
        public int Quantidade { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public long IdEmpresa { get; set; }
        public string IdUsuarioAjuste { get; set; }
    }
}
