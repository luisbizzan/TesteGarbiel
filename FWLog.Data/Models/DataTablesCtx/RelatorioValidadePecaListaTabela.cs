using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RelatorioValidadePecaListaTabela
    {
        [Display(Name = "Lote")]
        public long IdLote { get; set; }
        [Display(Name = "Referência")]
        public string ReferenciaProduto { get; set; }
        [Display(Name = "Descrição")]
        public string DescricaoProduto { get; set; }
        [Display(Name = "Data de Validade")]
        public string DataValidade { get; set; }
        public int Saldo { get; set; }


    }
}
