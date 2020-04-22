using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;

namespace FWLog.Web.Backoffice.Models.CaixaCtx
{
    public class CaixaListaViewModel
    {
        public CaixaListaViewModel()
        {
            ItemVazio = new CaixaListaTabela();
            Filtros = new CaixaListaFiltro();
        }

        public CaixaListaTabela ItemVazio { get; set; }
        public CaixaListaFiltro Filtros { get; set; }
    }

    //public class CaixaListaFilterViewModel
    //{
    //    [Display(Name = "Nome da Caixa")]
    //    public string Nome { get; set; }

    //    [Display(Name = "Texto Etiqueta")]
    //    public string TextoEtiqueta { get; set; }

    //    [Display(Name = "Peso Máximo")]
    //    public decimal PesoMaximo { get; set; }

    //    [Display(Name = "Cubicagem")]
    //    public int Cubagem { get; set; }

    //    [Display(Name = "Sobra")]
    //    public decimal Sobra { get; set; }

    //    [Display(Name = "Caixa para")]
    //    public CaixaTipoEnum IdCaixaTipo { get; set; }

    //    [Display(Name = "Peso da Caixa")]
    //    public decimal PesoCaixa { get; set; }

    //    [Display(Name = "Prioridade")]
    //    public int Prioridade { get; set; }

    //    [Display(Name = "Status da Caixa")]
    //    public bool Ativo { get; set; }
    //}

    //public class CaixaListaItemViewModel
    //{
    //    public long IdCaixa { get; set; }

    //    [Display(Name = "Nome da Caixa")]
    //    public string Nome { get; set; }

    //    [Display(Name = "Texto Etiqueta")]
    //    public string TextoEtiqueta { get; set; }

    //    [Display(Name = "Largura")]
    //    public int Largura { get; set; }

    //    [Display(Name = "Altura")]
    //    public int Altura { get; set; }

    //    [Display(Name = "Comprimento")]
    //    public int Comprimento { get; set; }

    //    [Display(Name = "Peso Máximo")]
    //    public decimal PesoMaximo { get; set; }

    //    [Display(Name = "Cubicagem")]
    //    public int Cubagem { get; set; }

    //    [Display(Name = "Sobra")]
    //    public decimal Sobra { get; set; }

    //    [Display(Name = "Caixa para")]
    //    public CaixaTipoEnum IdCaixaTipo { get; set; }

    //    [Display(Name = "Peso da Caixa")]
    //    public decimal PesoCaixa { get; set; }

    //    [Display(Name = "Prioridade")]
    //    public int Prioridade { get; set; }

    //    [Display(Name = "Status da Caixa")]
    //    public bool Ativo { get; set; }
    //}
}