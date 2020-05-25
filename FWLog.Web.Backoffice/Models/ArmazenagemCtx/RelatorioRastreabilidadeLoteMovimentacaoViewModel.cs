using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioRastreabilidadeLoteMovimentacaoViewModel
    {
        public RelatorioRastreabilidadeLoteMovimentacaoListItemViewModel EmptyItem { get; set; }

        public RelatorioRastreabilidadeLoteMovimentacaoFilterViewModel Filter { get; set; }

        public RelatorioRastreabilidadeLoteMovimentacaoViewModel()
        {
            EmptyItem = new RelatorioRastreabilidadeLoteMovimentacaoListItemViewModel();
            Filter = new RelatorioRastreabilidadeLoteMovimentacaoFilterViewModel();
        }
    }

    public class RelatorioRastreabilidadeLoteMovimentacaoListItemViewModel
    {
        public long? IdProduto { get; set; }

        public long IdLote { get; set; }

        [Display(Name = "Referência")]
        public string ReferenciaProduto { get; set; }

        [Display(Name = "Descrição")]
        public string DescricaoProduto { get; set; }

        public string Tipo { get; set; }

        public string Quantidade { get; set; }

        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [Display(Name = "Data e Hora")]
        public string DataHora { get; set; }

        [Display(Name = "Usuário")]
        public string UsuarioMovimentacao { get; set; }
    }

    public class RelatorioRastreabilidadeLoteMovimentacaoFilterViewModel
    {
        public long IdLote { get; set; }

        public long IdEmpresa { get; set; }

        public long IdProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public string UserNameMovimentacao { get; set; }

        [Display(Name = "Usuário")]
        public string IdUsuarioMovimentacao { get; set; }

        [Display(Name = "Tipo Movimentação")]
        public int? IdLoteMovimentacaoTipo { get; set; }

        [Display(Name = "Quantidade Inicial")]
        public int? QuantidadeInicial { get; set; }

        [Display(Name = "Quantidade Final")]
        public int? QuantidadeFinal { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? DataHoraInicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? DataHoraFinal { get; set; }

        public SelectList ListaLoteMovimentacaoTipo { get; set; }
    }
}