using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.FilterCtx
{
    public class RelatorioValidadePecaListaFiltro
    {
        public long IdEmpresa { get; set; }

        [Display(Name = "Lote")]
        public long? IdLote { get; set; }

        [Display(Name = "Produto")]
        public long? IdProduto { get; set; }

        public string DescricaoProduto { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? DataFinal { get; set; }
    }
}
