using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.RecebimentoEtiquetaIndividualPersonalizadaCtx
{
    public class RecebimentoEtiquetaIndividualPersonalizadaListViewModel
    {
        public RecebimentoEtiquetaIndividualPersonalizadaListViewModel()
        {
            EmptyItem = new RecebimentoEtiquetaIndividualPersonalizadaListItemViewModel();
            Filter = new RecebimentoEtiquetaIndividualPersonalizadaFilterViewModel();
        }

        public RecebimentoEtiquetaIndividualPersonalizadaListItemViewModel EmptyItem { get; set; }
        public RecebimentoEtiquetaIndividualPersonalizadaFilterViewModel Filter { get; set; }
    }


    public class RecebimentoEtiquetaIndividualPersonalizadaListItemViewModel
    {
        public long IdLogEtiquetagem { get; set; }

        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Tipo")]
        public string TipoEtiquetagem { get; set; }

        [Display(Name = "Quantidade")]
        public long Quantidade { get; set; }

        [Display(Name = "Impresso em")]
        public string DataHora { get; set; }

        [Display(Name = "Usuário")]
        public string Usuario { get; set; }
    }

    public class RecebimentoEtiquetaIndividualPersonalizadaFilterViewModel
    {
        public int? TipoEtiquetagem { get; set; }

        [Display(Name = "Referência")]
        public long? IdProduto { get; set; }
        public string DescricaoProduto { get; set; }

        [Display(Name = "Data de Impressão Inicial")]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data de Impressão Final")]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Quantidade Inicial")]
        public int? QuantidadeInicial { get; set; }

        [Display(Name = "Quantidade Final")]
        public int? QuantidadeFinal { get; set; }

        [Display(Name = "Impresso por")]
        public string IdUsuarioEtiquetagem { get; set; }
        public string UsuarioEtiquetagem { get; set; }

        public SelectList ListaTipoEtiquetagem { get; set; }
    }
}