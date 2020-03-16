namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class BOImprimirDevolucaoTotalViewModel
    {
        public string  NumeroNF     { get; set; }
        public string  Serie        { get; set; }
        public long    IdLote       { get; set; }
        public long    IdNotaFiscal { get; set; }
        public string  Usuario      { get; set; }
        public string  Senha        { get; set; }
    }                                                            
}