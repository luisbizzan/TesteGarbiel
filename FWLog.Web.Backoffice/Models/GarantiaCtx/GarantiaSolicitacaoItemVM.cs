using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaSolicitacaoItemVM
    {
        public GarantiaSolicitacaoListVM Solicitacao { get; set; }
        public List<GarantiaSolicitacaoItemListVM> Itens { get; set; }
        public GarantiaSolicitacaoItemListVM ItensCabecalho { get; set; }

        public GarantiaSolicitacaoItemVM()
        {
            Solicitacao = new GarantiaSolicitacaoListVM();
            Itens = new List<GarantiaSolicitacaoItemListVM>();
            ItensCabecalho = new GarantiaSolicitacaoItemListVM();
        }
    }

    public class GarantiaSolicitacaoItemListVM
    {
        [Display(Name = "Refx")]
        public string Refx { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        public long Id { get; set; }

        public long Id_solicitacao { get; set; }

        [Display(Name = "Quant")]
        public long Quant { get; set; }

        [Display(Name = "Valor")]
        public long Valor { get; set; }

        [Display(Name = "Valor Total")]
        public long Valor_Total { get; set; }

        [Display(Name = "Fornecedor")]
        public String Cod_Fornecedor { get; set; }
    }
}