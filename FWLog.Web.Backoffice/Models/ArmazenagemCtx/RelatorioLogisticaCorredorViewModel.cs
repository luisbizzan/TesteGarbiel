using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioLogisticaCorredorViewModel
    {
        public RelatorioLogisticaCorredorListItemViewModel EmptyItem { get; set; }

        public RelatorioLogisticaCorredorFilterViewModel Filter { get; set; }


        public RelatorioLogisticaCorredorViewModel()
        {
            EmptyItem = new RelatorioLogisticaCorredorListItemViewModel();
            Filter = new RelatorioLogisticaCorredorFilterViewModel();
        }
    }

    public class RelatorioLogisticaCorredorListItemViewModel
    {
        [Display(Name = "Endereço")]
        public string Codigo { get; set; }

        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Tipo")]
        public string Tipo { get; set; }

        [Display(Name = "Comprimento")]
        public string Comprimento { get; set; }

        [Display(Name = "Largura")]
        public string Largura { get; set; }

        [Display(Name = "Altura")]
        public string Altura { get; set; }

        [Display(Name = "Cubagem")]
        public string Cubagem { get; set; }

        [Display(Name = "Giro 6m")]
        public string Giro6m { get; set; }

        [Display(Name = "Giro DD")]
        public string GiroDD { get; set; }

        [Display(Name = "It.loc")]
        public string ItLoc { get; set; }

        [Display(Name = "Saldo")]
        public string Saldo { get; set; }

        [Display(Name = "Dura DD")]
        public string DuraDD { get; set; }

        [Display(Name = "Dt.repo")]
        public string DtRepo { get; set; }
    }

    public class RelatorioLogisticaCorredorFilterViewModel
    {
        [Required]
        [Display(Name = "Nível de Armazenagem")]
        public long IdNivelArmazenagem { get; set; }
        public string DescricaoNivelArmazenagem { get; set; }

        [Required]
        [Display(Name = "Ponto de Armazenagem")]
        public long IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }

        [Display(Name = "Corredor Inicial")]
        public int? CorredorInicial { get; set; }

        [Display(Name = "Corredor Final")]
        public int? CorredorFinal { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Ordenação")]
        public int Ordenacao { get; set; }
    }
}