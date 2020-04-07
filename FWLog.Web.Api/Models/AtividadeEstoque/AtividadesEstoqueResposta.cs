using FWLog.Data.Models.DataTablesCtx;
using System.Collections.Generic;

namespace FWLog.Web.Api.Models.AtividadeEstoque
{
    public class AtividadesEstoqueResposta
    {
        public int CorredorInicio { get; set; }
        public int CorredorFim { get; set; }
        public List<AtividadeEstoqueListaLinhaTabela> Lista { get; set; }
    }
}