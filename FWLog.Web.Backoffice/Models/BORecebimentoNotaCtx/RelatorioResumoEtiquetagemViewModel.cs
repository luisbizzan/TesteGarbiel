using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class RelatorioResumoEtiquetagemViewModel
    {
        public RelatorioResumoEtiquetagemViewModel()
        {
            EmptyItem = new RelatorioResumoEtiquetagemListItemViewModel();
            Filter = new RelatorioResumoEtiquetagemFilterViewModel();
        }

        public RelatorioResumoEtiquetagemListItemViewModel EmptyItem { get; set; }
        public RelatorioResumoEtiquetagemFilterViewModel Filter { get; set; }
    }

    public class RelatorioResumoEtiquetagemListItemViewModel
    {
        [Display(Name = "Id")]
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
        public string Fornecedor { get; set; }
    }

    public class RelatorioResumoEtiquetagemFilterViewModel
    {
        [Display(Name = "Tipo de Etiqueta")]
        public int? IdTipoEtiquetagem { get; set; }

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

        [Display(Name = "Usuário")]
        public string IdUsuarioEtiquetagem { get; set; }
        public string UsuarioEtiquetagem { get; set; }

        [Display(Name = "Fornecedor")]
        public long? IdFornecedor { get; set; }
        public string NomeFantasiaFornecedor { get; set; }

        public SelectList ListaTipoEtiquetagem { get; set; }
    }
}