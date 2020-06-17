using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.CaixaRecusaCtx
{
    public class CaixaRecusaListaViewModel
    {
        public CaixaRecusaListaViewModel()
        {
            ItemVazio = new CaixaRecusaListaTabela();
            Filtros = new CaixaRecusaListaFiltro();
        }

        public CaixaRecusaListaTabela ItemVazio { get; set; }
        public CaixaRecusaListaFiltro Filtros { get; set; }

        public SelectList ListaStatus { get; set; }
        public SelectList ListaCaixaTipo { get; set; }
    }
}