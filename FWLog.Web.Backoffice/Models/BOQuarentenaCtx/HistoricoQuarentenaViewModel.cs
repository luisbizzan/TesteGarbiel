using System.Collections.Generic;

namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class HistoricoQuarentenaViewModel
    {

        public List<HistoricoQuarentenaItemViewModel> Itens { get; set; } = new List<HistoricoQuarentenaItemViewModel>();
    }

    public class HistoricoQuarentenaItemViewModel
    {
        public string Data { get; set; }

        public string Usuario { get; set; }

        public string Descricao { get; set; }
    }

}