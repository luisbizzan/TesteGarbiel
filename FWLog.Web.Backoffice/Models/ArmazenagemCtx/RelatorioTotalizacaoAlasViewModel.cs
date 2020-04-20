using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioTotalizacaoAlasViewModel
    {
        public RelatorioTotalizacaoAlasListItemViewModel EmptyItem { get; set; }

        public RelatorioTotalizacaoAlasFilterViewModel Filter { get; set; }

        public RelatorioTotalizacaoAlasViewModel()
        {
            EmptyItem = new RelatorioTotalizacaoAlasListItemViewModel();
            Filter = new RelatorioTotalizacaoAlasFilterViewModel();
        }
    }

    public class RelatorioTotalizacaoAlasListItemViewModel
    {
        [Display(Name = "")]
        public string NumeroCorredor { get; set; }

        [Display(Name = "Endereço")]
        public string CodigoEndereco { get; set; }

        [Display(Name = "Usuário")]
        public string IdUsuarioInstalacao { get; set; }

        [Display(Name = "Referência")]
        public string ReferenciaProduto { get; set; }

        [Display(Name = "Lote")]
        public string IdLote { get; set; }

        [Display(Name = "Data")]
        public string DataInstalacao { get; set; }

        [Display(Name = "Peso")]
        public string PesoProduto { get; set; }

        [Display(Name = "Quantidade")]
        public string QuantidadeProdutoPorEndereco { get; set; }

        [Display(Name = "Peso Total")]
        public string PesoTotalDeProduto { get; set; }
    }

    public class RelatorioTotalizacaoAlasFilterViewModel
    {
        [Display(Name = "Corredor Inicial")]
        public int? CorredorInicial { get; set; }

        [Display(Name = "Corredor Final")]
        public int? CorredorFinal { get; set; }

        public IEnumerable<long> ListaIdEnderecoArmazenagem { get; set; }

        [Display(Name = "Imprimir Vazia?")]
        public bool ImprimirVazia { get; set; }

        [Required]
        [Display(Name = "Nível de Armazenagem")]
        public long? IdNivelArmazenagem { get; set; }
        public string DescricaoNivelArmazenagem { get; set; }

        [Required]
        [Display(Name = "Ponto de Armazenagem")]
        public long? IdPontoArmazenagem { get; set; }
        public string DescricaoPontoArmazenagem { get; set; }

    }
}