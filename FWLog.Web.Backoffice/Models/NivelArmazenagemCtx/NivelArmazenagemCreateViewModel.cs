namespace FWLog.Web.Backoffice.Models.NivelArmazenagemCtx
{
    public class NivelArmazenagemCreateViewModel
    {
        public long IdNivelArmazenagem { get; set; }

        public long IdEmpresa { get; set; }

        public string Descricao { get; set; }

        public bool Ativo { get; set; }

    }
}