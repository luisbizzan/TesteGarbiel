using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Models.CaixaRecusaCtx
{
    public class CaixaRecusaEdicaoViewModel
    {
        public CaixaRecusaEdicaoViewModel()
        {
            Lista = new List<CaixaRecusaEdicaoItemViewModel>();
        }
        
        public long IdEmpresa { get; set; }

        public long? IdProduto { get; set; }
        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

        public long? IdCaixa { get; set; }
        [Display(Name = "Caixa")]
        public string DescricaoCaixa { get; set; }

        public List<CaixaRecusaEdicaoItemViewModel> Lista { get; set; }
    }

    public class CaixaRecusaEdicaoItemViewModel
    {
        public long IdEmpresa { get; set; }

        public long? IdProduto { get; set; }
        [Display(Name = "Produto")]
        public string DescricaoProduto { get; set; }

        public long? IdCaixa { get; set; }
        [Display(Name = "Caixa")]
        public string DescricaoCaixa { get; set; }
    }
}