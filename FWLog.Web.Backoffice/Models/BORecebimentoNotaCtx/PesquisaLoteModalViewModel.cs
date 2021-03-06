﻿using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class PesquisaLoteModalViewModel
    {
        public PesquisaLoteModalItemViewModel EmptyItem { get; set; }
        public PesquisaLoteModalFilterViewModel Filter { get; set; }

        public PesquisaLoteModalViewModel()
        {
            EmptyItem = new PesquisaLoteModalItemViewModel();
            Filter = new PesquisaLoteModalFilterViewModel();
        }
    }

    public class PesquisaLoteModalItemViewModel
    {
        [Display(Name = "Lote")]
        public long? NroLote { get; set; }

        [Display(Name = "Nota Fiscal")]
        public int NroNota { get; set; }

        [Display(Name = "Data Recebimento")]
        public string Recebimento { get; set; }

        [Display(Name = "Fornecedor")]
        public string NomeFantasiaFormecedor { get; set; }
    }

    public class PesquisaLoteModalFilterViewModel
    {
        [Display(Name = "Lote")]
        public long? NroLote { get; set; }

        [Display(Name = "Nota Fiscal")]
        public int? NroNota { get; set; }

        [Display(Name = "Código do Fornecedor")]
        public long? CodFornecesor { get; set; }

        [Display(Name = "CNPJ do Fornecedor")]
        public string CNPJFornecedor { get; set; }

        [Display(Name = "Data Recebimento")]
        public DateTime? Recebimento { get; set; }
    }
}