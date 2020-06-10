using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class CaixaRecusaListaTabela
    {
        public long IdEmpresa { get; set; }

        public long IdCaixa { get; set; }
        [Display(Name ="Caixa")]
        public string NomeCaixa { get; set; }
    }
}
