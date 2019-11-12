using System.Collections.Generic;

namespace FWLog.Web.Backoffice.Models.BOPrinterCtx
{
    public class BOPrinterSelecionarViewModel
    {
        public List<BOPrinterSelecionarImpressoraViewModel> Impressoras { get; set; }
    }

    public class BOPrinterSelecionarImpressoraViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
    }
}