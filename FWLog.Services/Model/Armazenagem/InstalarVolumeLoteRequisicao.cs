namespace FWLog.Services.Model.Armazenagem
{
    public class InstalarVolumeLoteRequisicao
    {
        public long IdLote { get; set; }
        public long IdProduto { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeCaixas { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public long IdEmpresa { get; set; }
        public string IdUsuarioInstalacao { get; set; }
        public bool PermissaoInstalarForaMultiplo { get; set; }
        public string UsuarioPermissaoInstalarForaMultiplo { get; set; }
    }
}