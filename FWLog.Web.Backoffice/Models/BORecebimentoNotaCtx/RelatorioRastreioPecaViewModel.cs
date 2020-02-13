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
        [Display(Name = "Número Empresa")]
        public long IdEmpresa { get; set; }

        [Display(Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(Name = "Número Lote")]
        public long IdLote { get; set; }

        [Display(Name = "Número Nota")]
        public int NroNota { get; set; }

        [Display(Name = "Referência do Produto")]
        public string ReferenciaPronduto { get; set; }

        [Display(Name = "Recebimento")]
        public string DataRecebimento { get; set; }

        [Display(Name = "Quantidade Compra")]
        public long? QtdCompra { get; set; }

        [Display(Name = "Quantidade Recebida")]
        public long? QtdRecebida { get; set; }
    }

    public class RelatorioRastreioPecaFilterViewModel : IRelatorioRastreioPecaListaFiltro
    {
        public long? IdProduto { get; set; }
        public long IdEmpresa { get; set; }

        [Display(Name = "Número Lote")]
        public long? IdLote { get; set; }

        [Display(Name = "Número Nota")]
        public int? NroNota { get; set; }

        [Display(Name = "Referência do Produto")]
        public string DescricaoProduto { get; set; }

        [Display(Name = "Data")]
        public DateTime? DataCompraMinima { get; set; }

        [Display(Name = "Data")]
        public DateTime? DataCompraMaxima { get; set; }

        [Display(Name = "Data")]
        public DateTime? DataRecebimentoMinima { get; set; }

        [Display(Name = "Data")]
        public DateTime? DataRecebimentoMaxima { get; set; }

        [Display(Name = "Quantidade")]
        public long? QtdCompraMinima { get; set; }

        [Display(Name = "Quantidade")]
        public long? QtdCompraMaxima { get; set; }

        [Display(Name = "Quantidade")]
        public long? QtdRecebidaMinima { get; set; }

        [Display(Name = "Quantidade")]
        public long? QtdRecebidaMaxima { get; set; }
    }
}