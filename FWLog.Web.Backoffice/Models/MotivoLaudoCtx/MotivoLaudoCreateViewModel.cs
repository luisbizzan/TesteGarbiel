using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.MotivoLaudoCtx
{
    public class MotivoLaudoCreateViewModel
    {
        public long IdMotivoLaudo { get; set; }
        [Display(Name = "Descrição")]
        [Required]
        public string Descricao { get; set; }
        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }
    }
}