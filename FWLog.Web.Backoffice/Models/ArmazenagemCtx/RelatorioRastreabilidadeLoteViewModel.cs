using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioRastreabilidadeLoteViewModel
    {
        public RelatorioRastreabilidadeLoteListItemViewModel EmptyItem { get; set; }

        public RelatorioRastreabilidadeLoteFilterViewModel Filter { get; set; }

        public RelatorioRastreabilidadeLoteViewModel()
        {
            EmptyItem = new RelatorioRastreabilidadeLoteListItemViewModel();
            Filter = new RelatorioRastreabilidadeLoteFilterViewModel();
        }
    }

    public class RelatorioRastreabilidadeLoteListItemViewModel
    {
        [Display(Name = "Lote")]
        public long IdLote { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Recebido em")]
        public string DataRecebimento { get; set; }

        [Display(Name = "Conferido em")]
        public string DataConferencia { get; set; }

        [Display(Name = "Qtde. Volumes")]
        public string QuantidadeVolume { get; set; }

        [Display(Name = "Qtde. Peças")]
        public string QuantidadePeca { get; set; }
    }

    public class RelatorioRastreabilidadeLoteFilterViewModel
    {
        [Display(Name = "Lote")]
        public long? IdLote { get; set; }

        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

        public long? IdProduto { get; set; }

        public long IdEmpresa { get; set; }
    }
}