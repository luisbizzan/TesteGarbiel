﻿using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.BOPrinterCtx
{
    public class BOPrinterSelecionarViewModel
    {
        public string ImpressaoItemDescricao { get; set; }
        public string Acao { get; set; }
        public string Id { get; set; }
        public SelectList Impressoras { get; set; }
    }
}