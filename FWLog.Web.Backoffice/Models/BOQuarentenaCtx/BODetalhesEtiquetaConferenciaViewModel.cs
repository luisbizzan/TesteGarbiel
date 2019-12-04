using System;

namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class DetalhesQuarentenaViewModel
    {
        public long IdQuarentena { get; set; }

        public string DataAbertura { get; set; }

        public string DataEncerramento { get; set; }

        public long IdStatus { get; set; }

        public string Observacao { get; set; }

        public bool PermiteEdicao
        {
            get
            {
                long[] statusNaoPermite = new long[] { 4, 5 };

                return !Array.Exists(statusNaoPermite, x => x == IdStatus);
            }
        }
    }
}