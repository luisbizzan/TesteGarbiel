using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.CaixaCtx
{
    public class CaixaSearchModalViewModel
    {
        public CaixaSearchModalViewModel()
        {
            ItemVazio = new CaixaListaTabela();
            Filtros = new CaixaListaFiltro();
        }

        public CaixaListaTabela ItemVazio { get; set; }
        public CaixaListaFiltro Filtros { get; set; }

        public SelectList ListaStatus { get; set; }
        public SelectList ListaCaixaTipo { get; set; }
    }
}