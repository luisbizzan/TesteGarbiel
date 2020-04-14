using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioTotalizacaoLocalizacaoViewModel
    {
        public RelatorioTotalizacaoLocalizacaoViewModel()
        {
            EmptyItem = new RelatorioTotalizacaoLocalizacaoListItemViewModel();
            Filter = new RelatorioTotalizacaoLocalizacaoFilterViewModel();
        }

        public RelatorioTotalizacaoLocalizacaoListItemViewModel EmptyItem { get; set; }
        public RelatorioTotalizacaoLocalizacaoFilterViewModel Filter { get; set; }
    }

    public class RelatorioTotalizacaoLocalizacaoListItemViewModel
    {
        [Display(Name = "Endereço")]
        public string CodigoEndereco { get; set; }

        [Display(Name = "Referência")]
        public string ReferenciaProduto { get; set; }

        [Display(Name = "UN")]
        public string Unidade { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }
    }

    public class RelatorioTotalizacaoLocalizacaoFilterViewModel
    {
        [Required]
        [Display(Name = "Nível de Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }

        public string DescricaoNivelArmazenagem { get; set; }

        [Required]
        [Display(Name = "Ponto de Armazenagem")]
        public long? IdPontoArmazenagem { get; set; }

        public string DescricaoPontoArmazenagem { get; set; }

        [Display(Name = "Corredor Inicial")]
        public int? CorredorInicial { get; set; }

        [Display(Name = "Corredor Final")]
        public int? CorredorFinal { get; set; }
    }
}