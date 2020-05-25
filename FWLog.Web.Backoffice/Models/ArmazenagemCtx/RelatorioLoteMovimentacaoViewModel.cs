using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioLoteMovimentacaoViewModel
    {
        public RelatorioLoteMovimentacaoListItemViewModel EmptyItem { get; set; }

        public RelatorioLoteMovimentacaoFilterViewModel Filter { get; set; }

        public RelatorioLoteMovimentacaoViewModel()
        {
            EmptyItem = new RelatorioLoteMovimentacaoListItemViewModel();
            Filter = new RelatorioLoteMovimentacaoFilterViewModel();
        }
    }

    public class RelatorioLoteMovimentacaoListItemViewModel
    {
        public long? IdProduto { get; set; }

        [Display(Name = "Lote")]
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

    public class RelatorioLoteMovimentacaoFilterViewModel
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Lote")]
        public long? IdLote { get; set; }

        [Display(Name = "Produto")]
        public long? IdProduto { get; set; }

        public string DescricaoProduto { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? DataHoraInicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? DataHoraFinal { get; set; }

        [Display(Name = "Usuário")]
        public string IdUsuarioMovimentacao { get; set; }

        public string UserNameMovimentacao { get; set; }

        [Display(Name = "Endereço")]
        public long? IdEnderecoArmazenagem { get; set; }

        public string CodigoEnderecoArmazenagem { get; set; }

        [Display(Name = "Tipo Movimentação")]
        public int? IdLoteMovimentacaoTipo { get; set; }

        public SelectList ListaLoteMovimentacaoTipo { get; set; }
    }
}