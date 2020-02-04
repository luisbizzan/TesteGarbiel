using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.NivelArmazenagemCtx
{
    public class NivelArmazenagemCreateViewModel
    {
        public long IdNivelArmazenagem { get; set; }

        public long IdEmpresa { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }
    }
}