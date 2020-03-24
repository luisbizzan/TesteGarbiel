using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Model.Relatorios
{
    public class ImprimirRelatorioProdutosRequest
    {
        public long IdImpressora { get; set; }
        public long? IdEmpresa { get; set; }
        public string NomeUsuario { get; set; }
        public string Referencia { get; set; }
        public string Descricao { get; set; }
        public string CodigoDeBarras { get; set; }
        public int? ProdutoStatus { get; set; }
    }
}
