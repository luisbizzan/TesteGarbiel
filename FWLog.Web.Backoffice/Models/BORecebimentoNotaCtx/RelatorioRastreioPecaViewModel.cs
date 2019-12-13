using FWLog.Data.Models.FilterCtx;
using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class RelatorioRastreioPecaViewModel
    {
        public RelatorioRastreioPecaListItemViewModel EmptyItem { get; set; }

        public RelatorioRastreioPecaFilterViewModel Filter { get; set; }

        public RelatorioRastreioPecaViewModel()
        {
            EmptyItem = new RelatorioRastreioPecaListItemViewModel();
            Filter = new RelatorioRastreioPecaFilterViewModel();
        }
    }

    public class RelatorioRastreioPecaListItemViewModel
    {
        [Display(Name = "Nro. Empresa")]
        public long IdEmpresa { get; set; }

        [Display(Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(Name = "Nro. Lote")]
        public long IdLote { get; set; }

        [Display(Name = "Nro. Nota")]
        public int NroNota { get; set; }

        [Display(Name = "Referência do Pronduto")]
        public string ReferenciaPronduto { get; set; }

        [Display(Name = "Recebimento")]
        public DateTime DataRecebimento { get; set; }

        [Display(Name = "Qtd. Compra")]
        public long? QtdCompra { get; set; }

        [Display(Name = "Qtd. Recebida")]
        public long? QtdRecebida { get; set; }
    }

    public class RelatorioRastreioPecaFilterViewModel : IRelatorioRastreioPecaListaFiltro
    {
        public long IdEmpresa { get; set; }
        public long? IdLote { get; set; }
        public int? NroNota { get; set; }
        public string ReferenciaPronduto { get; set; }
        public DateTime? DataCompraMinima { get; set; }
        public DateTime? DataCompraMaxima { get; set; }
        public DateTime? DataRecebimentoMinima { get; set; }
        public DateTime? DataRecebimentoMaxima { get; set; }
        public long? QtdCompraMinima { get; set; }
        public long? QtdCompraMaxima { get; set; }
        public long? QtdRecebidaMinima { get; set; }
        public long? QtdRecebidaMaxima { get; set; }
    }
}