using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class RelatorioValidadePecaViewModel
    {
        public RelatorioValidadePecaViewModel()
        {
            ItemVazio = new RelatorioValidadePecaListaTabela();
            Filtros = new RelatorioValidadePecaListaFiltro();
        }

        public RelatorioValidadePecaListaTabela ItemVazio { get; set; }
        public RelatorioValidadePecaListaFiltro Filtros { get; set; }
    }
}