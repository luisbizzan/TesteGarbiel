using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.ProdutoCtx
{
    public class ProdutoSearchModalViewModel
    {
        public ProdutoSearchModalItemViewModel EmptyItem { get; set; }
        public ProdutoSearchModalFilterViewModel Filter { get; set; }

        public ProdutoSearchModalViewModel()
        {
            EmptyItem = new ProdutoSearchModalItemViewModel();
            Filter = new ProdutoSearchModalFilterViewModel();
        }
    }

    public class ProdutoSearchModalItemViewModel
    {
        public long IdProduto { get; set; }

        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
    }

    public class ProdutoSearchModalFilterViewModel
    {
        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Status")]
        public bool? Status { get; set; }
        
        public long? IdLote { get; set; }

        public long? IdPedidoVendaVolume { get; set; }

        public SelectList ListaStatus { get; set; }
    }
}