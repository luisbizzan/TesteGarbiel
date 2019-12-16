namespace FWLog.Data.Models.FilterCtx
{
    public class UsuarioListaFiltro
    {
        public long IdEmpresa { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public bool? Ativo { get; set; }
    }
}
