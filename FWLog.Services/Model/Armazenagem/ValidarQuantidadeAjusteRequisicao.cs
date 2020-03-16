namespace FWLog.Services.Model.Armazenagem
{
    public class ValidarQuantidadeAjusteRequisicao
    {
        public long IdEmpresa { get; set; }
        public long IdLote { get; set; }
        public long IdProduto { get; set; }
        public int Quantidade { get; set; }
    }
}
