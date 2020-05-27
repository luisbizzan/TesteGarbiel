using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class MovimentacaoVolumesViewModel
    {
        public MovimentacaoVolumesViewModel()
        {
            Filter = new MovimentacaoVolumesFilterViewModel();
            Items = new List<MovimentacaoVolumesListItemViewModel>();
        }

        [Display(Name = "Aguardando integração")]
        public int? AguardandoIntegracao { get; set; }

        [Display(Name = "Integrado OK")]
        public int? AguardandoRobo { get; set; }

        public MovimentacaoVolumesFilterViewModel Filter { get; set; }
        public List<MovimentacaoVolumesListItemViewModel> Items { get; set; }
    }

    public class MovimentacaoVolumesFilterViewModel
    {
        [Display(Name = "Data Inicial")]
        [Required]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final")]
        [Required]
        public DateTime? DataFinal { get; set; }
    }

    public class MovimentacaoVolumesListItemViewModel
    {
        [Display(Name = "")]
        public string Corredores { get; set; }

        public string PontoArmazenagemDescricao { get; set; }

        [Display(Name = "Enviado Separação")]
        public int EnviadoSeparacao { get; set; }

        [Display(Name = "Em Separação")]
        public int EmSeparacao { get; set; }

        [Display(Name = "Finalizado Separação")]
        public int FinalizadoSeparacao { get; set; }

        [Display(Name = "Instalado Transportadora")]
        public int InstaladoTransportadora { get; set; }

        [Display(Name = "DOCA")]
        public int Doca { get; set; }

        [Display(Name = "Enviado Transportadora")]
        public int EnviadoTransportadora { get; set; }

        [Display(Name = "Total")]
        public int Total { get; set; }
    }
}