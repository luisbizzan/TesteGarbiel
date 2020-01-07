using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class QuarentenaHistoricoListRow
    {
        public long IdQuarentenaHistorico { get; set; }
        public long IdQuarentena { get; set; }
        public string Descricao { get; set; }
        public string IdUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public DateTime Data { get; set; }
    }
}
