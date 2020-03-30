using FWLog.Data.Models.DataTablesCtx;
using System.Collections.Generic;

namespace FWLog.Web.Api.Models.Armazenagem
{
    public class EnderecosProdutosResposta
    {
        public List<EnderecoProdutoListaLinhaTabela> Lista { get; set; }
    }
}