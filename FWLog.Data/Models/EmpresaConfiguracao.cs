using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models
{
    class EmpresaConfiguracao
    {
        [Key]
        public long IdEmpresaConfiguracao { get; set; }

        public long IdEmpresaMatriz { get; set; }

        public long IdEmpresaTipo { get; set; }

        public long IdEmpresa { get; set; }
    }
}
