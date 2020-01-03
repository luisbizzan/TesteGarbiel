using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class HistoricoQuarentenaViewModel
    {

        public List<HistoricoQuarentenaItemViewModel> Itens { get; set; } = new List<HistoricoQuarentenaItemViewModel>();
    }

    public class HistoricoQuarentenaItemViewModel
    {
        public string Data { get; set; }

        [Display(Name = "Usuário")]
        public string Usuario { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }

}