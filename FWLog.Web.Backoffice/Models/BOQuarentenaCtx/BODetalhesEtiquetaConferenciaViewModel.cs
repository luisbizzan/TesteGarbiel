using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class DetalhesQuarentenaViewModel
    {
        public long IdQuarentena { get; set; }

        [Display(Name = "Data de Abertura")]
        public string DataAbertura { get; set; }
        
        [Display(Name = "Data de Encerramento")]
        public string DataEncerramento { get; set; }

        [Display(Name = "Status")]
        public long IdStatus { get; set; }

        [Display(Name = "Observação")]
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