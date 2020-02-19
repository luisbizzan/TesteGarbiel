using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class MotivoLaudoListViewModel
    {
        public MotivoLaudoListItemViewModel EmptyItem { get; set; }
        public MotivoLaudoFilterViewModel Filter { get; set; }

        public MotivoLaudoListViewModel()
        {
            EmptyItem = new MotivoLaudoListItemViewModel();
            Filter = new MotivoLaudoFilterViewModel();
        }
    }

    public class MotivoLaudoListItemViewModel
    {
        [Display(Name = "Código")]
        public long IdMotivoLaudo { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Ativo")]
        public string Status { get; set; }
    }

    public class MotivoLaudoFilterViewModel
    {
        [Display(Name = "Código")]
        public long? IdMotivoLaudo { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Ativo")]
        public bool? Status { get; set; }
    }
}

    

    