namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class DetalhesQuarentenaViewModel
    {
        public long IdQuarentena { get; set; }

        public string DataAbertura { get; set; }

        public string DataEncerramento { get; set; }

        public long IdStatus { get; set; }

        public string Observacao { get; set; }
    }
}