using FWLog.Data.Models.DataTablesCtx;
using System.Collections.Generic;

namespace FWLog.Web.Api.Models.AtividadeEstoque
{
    public class AtividadesEstoqueResposta
    {
        public List<AtividadeEstoqueListaLinhaTabela> Lista { get; set; }
    }
}