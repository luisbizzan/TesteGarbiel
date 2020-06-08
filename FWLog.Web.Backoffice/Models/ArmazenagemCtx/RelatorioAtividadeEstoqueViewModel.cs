using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.ArmazenagemCtx
{
    public class RelatorioAtividadeEstoqueViewModel
    {
        public RelatorioAtividadeEstoqueListItemViewModel EmptyItem { get; set; }

        public RelatorioAtividadeEstoqueFilterViewModel Filter { get; set; }

        public RelatorioAtividadeEstoqueViewModel()
        {
            EmptyItem = new RelatorioAtividadeEstoqueListItemViewModel();
            Filter = new RelatorioAtividadeEstoqueFilterViewModel();
        }
    }

    public class RelatorioAtividadeEstoqueListItemViewModel
    {
        [Display(Name = "Tipo")]
        public string TipoAtividade { get; set; }

        [Display(Name = "Endereço")]
        public string CodigoEndereco { get; set; }

        [Display(Name = "Referência")]
        public string ReferenciaProduto { get; set; }

        [Display(Name = "Descrição")]
        public string DescricaoProduto { get; set; }

        [Display(Name = "Qtde. Inicial")]
        public string QuantidadeInicial { get; set; }

        [Display(Name = "Data Solic.")]
        public string DataSolicitacao { get; set; }

        [Display(Name = "Qtde. Final")]
        public string QuantidadeFinal { get; set; }

        [Display(Name = "Data Exec.")]
        public string DataExecucao { get; set; }

        [Display(Name = "Usuário")]
        public string UsuarioExecucao { get; set; }

        [Display(Name = "% Divergência")]
        public string PorcentagemDivergencia { get; set; }

        public string Finalizado { get; set; }
    }

    public class RelatorioAtividadeEstoqueFilterViewModel
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

        public long? IdProduto { get; set; }

        [Display(Name = "Tipo Atividade")]
        public int? IdAtividadeEstoqueTipo { get; set; }

        [Display(Name = "Quantidade Inicial")]
        public int? QuantidadeInicial { get; set; }

        [Display(Name = "Quantidade Final")]
        public int? QuantidadeFinal { get; set; }

        [Display(Name = "Data Inicial Solic.")]
        public DateTime? DataInicialSolicitacao { get; set; }

        [Display(Name = "Data Final Solic.")]
        public DateTime? DataFinalSolicitacao { get; set; }

        [Display(Name = "Data Inicial Exec.")]
        public DateTime? DataInicialExecucao { get; set; }

        [Display(Name = "Data Final Exec.")]
        public DateTime? DataFinalExecucao { get; set; }

        public string UserNameExecucao { get; set; }

        [Display(Name = "Usuário")]
        public string IdUsuarioExecucao { get; set; }

        public SelectList ListaAtividadeEstoqueTipo { get; set; }
    }
}