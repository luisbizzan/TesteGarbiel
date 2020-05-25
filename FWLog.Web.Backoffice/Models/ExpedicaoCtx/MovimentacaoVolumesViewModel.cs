using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ExpedicaoCtx
{
    public class MovimentacaoVolumesViewModel
    {
        public MovimentacaoVolumesViewModel()
        {
            EmptyItem = new MovimentacaoVolumesListItemViewModel();
            Filter = new MovimentacaoVolumesFilterViewModel();
        }

        public MovimentacaoVolumesListItemViewModel EmptyItem { get; set; }
        public MovimentacaoVolumesFilterViewModel Filter { get; set; }
    }

    public class MovimentacaoVolumesListItemViewModel
    {
        //[Display(Name = "Transportadora")]
        //public string Transportadora { get; set; }

        //[Display(Name = "Endereço")]
        //public string CodigoEndereco { get; set; }

        //[Display(Name = "Pedido")]
        //public string NumeroPedido { get; set; }

        //[Display(Name = "Volume")]
        //public string NumeroVolume { get; set; }
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
}