using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Model.Armazenagem
{
    public class InstalarVolumeLoteRequisicao
    {
        public long IdLote { get; set; }
        public long IdProduto { get; set; }
        public int Quantidade { get; set; }
        public long IdEnderecoArmazenagem { get; set; }
        public long IdEmpresa { get; set; }
        public string IdUsuarioInstalacao { get; set; }
    }
}
